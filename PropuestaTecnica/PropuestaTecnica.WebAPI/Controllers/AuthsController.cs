using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropuestaTecnica.Bussiness.Services.Auth;
using PropuestaTecnica.Common.DTOs.Auths;
using PropuestaTecnica.Common.Entities;
using PropuestaTecnica.DataAccess.Data;

namespace PropuestaTecnica.WebAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;

    public AuthsController(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        AppDbContext context)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Usuario registrado correctamente" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized("Credenciales inválidas");

        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
            return Unauthorized("Credenciales inválidas");

        var (accessToken, refreshToken) = await _tokenService.GenerateTokens(user);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            accessToken,
            refreshToken = refreshToken.Token
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshRequestDTO dto)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == dto.RefreshToken);

        if (storedToken == null)
            return Unauthorized("Refresh token inválido");

        if (storedToken.IsRevoked)
            return Unauthorized("Este refresh token ha sido revocado");

        if (storedToken.ExpiresAt < DateTime.UtcNow)
            return Unauthorized("El refresh token ha expirado");

        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        if (user == null)
            return Unauthorized("Usuario no encontrado");

        // Revocar el token actual
        storedToken.IsRevoked = true;

        // Nuevo token
        var (newAccessToken, newRefreshToken) = await _tokenService.GenerateTokens(user);

        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken.Token
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(RefreshRequestDTO dto)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == dto.RefreshToken);

        if (storedToken == null)
            return NotFound("Refresh token no encontrado");

        storedToken.IsRevoked = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Sesión cerrada correctamente" });
    }
}
