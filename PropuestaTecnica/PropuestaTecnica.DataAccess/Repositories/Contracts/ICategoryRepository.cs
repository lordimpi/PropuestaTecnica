using PropuestaTecnica.Common.Entities;

namespace PropuestaTecnica.DataAccess.Repositories.Contracts;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<bool> AddAsync(Category category);
    Task<bool> UpdateAsync(Category category);
    Task<bool> RemoveAsync(Category category);

    IQueryable<Category> GetQueryable();
}
