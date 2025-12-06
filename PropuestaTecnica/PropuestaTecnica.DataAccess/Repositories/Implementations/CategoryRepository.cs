using Microsoft.EntityFrameworkCore;
using PropuestaTecnica.Common.Entities;
using PropuestaTecnica.DataAccess.Data;
using PropuestaTecnica.DataAccess.Repositories.Contracts;

namespace PropuestaTecnica.DataAccess.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<bool> AddAsync(Category category)
    {
        if (category == null)
            return false;

        await _context.Categories.AddAsync(category);
        return true;
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        if (category == null)
            return false;

        _context.Categories.Update(category);
        return true;
    }

    public async Task<bool> RemoveAsync(Category category)
    {
        if (category == null)
            return false;

        _context.Categories.Remove(category);
        return true;
    }

    public IQueryable<Category> GetQueryable()
    {
        return _context.Categories.AsQueryable();
    }
}
