using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PropuestaTecnica.Bussiness.Services.Contracts;
using PropuestaTecnica.Common.DTOs.Incidents;
using PropuestaTecnica.DataAccess.UnitOfWork;
using PropuestaTecnica.DataAccess.Utilities.Extensions.Queryable;
using PropuestaTecnica.WebAPI.Models;
using System.Security.Claims;

namespace PropuestaTecnica.WebAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class IncidentsController : ControllerBase
{
    private readonly IIncidentService _incidentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputCacheStore _outputCacheStore;
    private const string cacheTagIncidents = "incidents";

    public IncidentsController(IIncidentService incidentService, IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore)
    {
        _incidentService = incidentService;
        _unitOfWork = unitOfWork;
        _outputCacheStore = outputCacheStore;
    }

    [HttpGet]
    [OutputCache(PolicyName = "largo", Tags = [cacheTagIncidents])]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest pagination)
    {
        var query = _unitOfWork.IncidentsRepository
            .GetQueryable()
            .WithDetails();

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(i => i.Id)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ProjectToType<IncidentResponseDTO>()
            .ToListAsync();

        var metadata = new PaginationMetadata
        {
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = total
        };

        return Ok(new ApiResponse<IEnumerable<IncidentResponseDTO>>
        {
            Success = true,
            Data = items,
            Pagination = metadata
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _incidentService.GetByIdAsync(id);

        if (result == null)
            return NotFound(ApiResponse<string>.Fail("Incidente no encontrado."));

        return Ok(ApiResponse<IncidentResponseDTO>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IncidentCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Datos inválidos."));

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        dto.UserId = userId;

        var created = await _incidentService.CreateAsync(dto);

        if (created == null)
            return BadRequest(ApiResponse<string>.Fail("No se pudo crear el incidente."));

        await ClearCacheAsync([cacheTagIncidents]);

        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<IncidentResponseDTO>.Ok(created, "Incidente creado correctamente."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] IncidentUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Datos inválidos."));

        var updated = await _incidentService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound(ApiResponse<string>.Fail("No se pudo actualizar el incidente."));

        await ClearCacheAsync([cacheTagIncidents]);

        return Ok(ApiResponse<string>.Ok("Incidente actualizado correctamente."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _incidentService.DeleteAsync(id);

        if (!deleted)
            return NotFound(ApiResponse<string>.Fail("No se pudo eliminar el incidente."));

        await ClearCacheAsync([cacheTagIncidents]);

        return Ok(ApiResponse<string>.Ok("Incidente eliminado correctamente."));
    }

    private async Task ClearCacheAsync(params string[] tags)
    {
        if (tags == null || tags.Length == 0) return;

        var tasks = tags.Select(tag => _outputCacheStore.EvictByTagAsync(tag, default).AsTask());
        await Task.WhenAll(tasks);
    }
}
