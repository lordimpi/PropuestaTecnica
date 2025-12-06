using Microsoft.EntityFrameworkCore;
using PropuestaTecnica.Common.Entities;
using PropuestaTecnica.DataAccess.Data;
using PropuestaTecnica.DataAccess.Repositories.Contracts;

namespace PropuestaTecnica.DataAccess.Repositories.Implementations;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;

    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Comment>> GetByIncidentIdAsync(int incidentId)
    {
        return await _context.Comments
            .Where(c => c.IncidentId == incidentId)
            //.Include(c => c.User)
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _context.Comments.FindAsync(id);
    }

    public async Task<bool> AddAsync(Comment comment)
    {
        if (comment == null)
            return false;

        await _context.Comments.AddAsync(comment);
        return true;
    }

    public async Task<bool> RemoveAsync(Comment comment)
    {
        if (comment == null)
            return false;

        _context.Comments.Remove(comment);
        return true;
    }

    public IQueryable<Comment> GetQueryable()
    {
        return _context.Comments.AsQueryable();
    }
}