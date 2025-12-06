using PropuestaTecnica.DataAccess.Repositories.Contracts;

namespace PropuestaTecnica.DataAccess.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IIncidentRepository IncidentsRepository { get; }
    ICategoryRepository CategoriesRepository { get; }
    ICommentRepository CommentsRepository { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}