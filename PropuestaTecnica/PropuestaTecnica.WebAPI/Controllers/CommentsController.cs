using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropuestaTecnica.Bussiness.Services.Contracts;
using PropuestaTecnica.Common.DTOs.Comments;
using PropuestaTecnica.DataAccess.UnitOfWork;
using PropuestaTecnica.DataAccess.Utilities.Extensions.Queryable;
using PropuestaTecnica.WebAPI.Models;
using System.Security.Claims;

namespace PropuestaTecnica.WebAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IUnitOfWork _unitOfWork;

    public CommentsController(ICommentService commentService, IUnitOfWork unitOfWork)
    {
        _commentService = commentService;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("{incidentId:int}")]
    public async Task<IActionResult> AddComment(int incidentId, [FromBody] CommentCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Datos inválidos."));

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        dto.UserId = userId;

        var created = await _commentService.AddCommentAsync(incidentId, dto);

        if (created == null)
            return NotFound(ApiResponse<string>.Fail("No se pudo agregar el comentario. El incidente no existe."));

        return Ok(ApiResponse<CommentResponseDTO>.Ok(created, "Comentario registrado correctamente."));
    }

    [HttpGet("{incidentId:int}")]
    public async Task<IActionResult> GetByIncident(int incidentId, [FromQuery] PaginationRequest pagination)
    {
        var query = _unitOfWork.CommentsRepository
            .GetQueryable()
            .Where(c => c.IncidentId == incidentId)
            .WithDetails();

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ProjectToType<CommentResponseDTO>()
            .ToListAsync();

        var metadata = new PaginationMetadata
        {
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = total
        };

        return Ok(new ApiResponse<IEnumerable<CommentResponseDTO>>
        {
            Success = true,
            Data = items,
            Pagination = metadata
        });
    }
}
