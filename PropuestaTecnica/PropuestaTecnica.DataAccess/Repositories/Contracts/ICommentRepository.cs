using PropuestaTecnica.Common.Entities;

namespace PropuestaTecnica.DataAccess.Repositories.Contracts;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetByIncidentIdAsync(int incidentId);
    Task<Comment?> GetByIdAsync(int id);
    Task<bool> AddAsync(Comment comment);
    Task<bool> RemoveAsync(Comment comment);

    IQueryable<Comment> GetQueryable();
}
