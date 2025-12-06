using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropuestaTecnica.Bussiness.Services.Contracts;
using PropuestaTecnica.Common.DTOs.Categories;
using PropuestaTecnica.WebAPI.Models;

namespace PropuestaTecnica.WebAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();

        return Ok(ApiResponse<IEnumerable<CategoryDTO>>.Ok(categories));
    }
}
