using Microsoft.EntityFrameworkCore.Storage;
using PropuestaTecnica.DataAccess.Data;
using PropuestaTecnica.DataAccess.Repositories.Contracts;

namespace PropuestaTecnica.DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public IIncidentRepository IncidentsRepository { get; }
    public ICategoryRepository CategoriesRepository { get; }
    public ICommentRepository CommentsRepository { get; }

    public UnitOfWork(AppDbContext context, IIncidentRepository incidentRepository, ICategoryRepository categoryRepository, ICommentRepository commentRepository)
    {
        _context = context;
        IncidentsRepository = incidentRepository;
        CategoriesRepository = categoryRepository;
        CommentsRepository = commentRepository;
    }


    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _currentTransaction ??= await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}
