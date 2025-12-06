using Mapster;
using PropuestaTecnica.Bussiness.Services.Contracts;
using PropuestaTecnica.Common.DTOs.Categories;
using PropuestaTecnica.DataAccess.UnitOfWork;

namespace PropuestaTecnica.Bussiness.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
    {
        var categories = await _unitOfWork.CategoriesRepository.GetAllAsync();
        return categories.Adapt<IEnumerable<CategoryDTO>>();
    }
}
