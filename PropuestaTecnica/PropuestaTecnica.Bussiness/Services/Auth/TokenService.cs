using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PropuestaTecnica.Common.Configurations;
using PropuestaTecnica.Common.Entities;
using PropuestaTecnica.Common.IOptionPattern;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PropuestaTecnica.Bussiness.Services.Auth;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;
    public TokenService(IGenericOptionsService<JwtSettings> genericOptionsService, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _jwtSettings = genericOptionsService.GetSnapshotOptions();
    }

    public async Task<(string accessToken, RefreshToken refreshToken)> GenerateTokens(ApplicationUser user)
    {

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!)
        };

        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Claims adicionales opcionales
        if (!string.IsNullOrEmpty(user.FullName))
        {
            authClaims.Add(new Claim("fullName", user.FullName));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes),
            claims: authClaims,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            UserId = user.Id
        };

        return (accessToken, refreshToken);
    }
}