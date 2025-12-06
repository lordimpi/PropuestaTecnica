using PropuestaTecnica.Common.DTOs.Categories;

namespace PropuestaTecnica.Bussiness.Services.Contracts;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTO>> GetAllAsync();
}
