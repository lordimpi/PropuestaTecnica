using Microsoft.EntityFrameworkCore;
using PropuestaTecnica.Common.Entities;
using PropuestaTecnica.Common.Enums;
using PropuestaTecnica.DataAccess.Data;
using PropuestaTecnica.DataAccess.Repositories.Contracts;

namespace PropuestaTecnica.DataAccess.Repositories.Implementations;

public class IncidentRepository : IIncidentRepository
{
    private readonly AppDbContext _context;

    public IncidentRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Incident> GetQueryable()
    {
        return _context.Incidents.AsQueryable();
    }

    public async Task<IEnumerable<Incident>> GetAllAsync()
    {
        return await _context.Incidents
            .Include(i => i.Category)
            //.Include(i => i.User)
            .ToListAsync();
    }

    public async Task<Incident?> GetByIdAsync(int id)
    {
        return await _context.Incidents.FindAsync(id);
    }

    public async Task<IEnumerable<Incident>> GetByStatusAsync(IncidentStatus status)
    {
        return await _context.Incidents
            .Where(i => i.Status == status)
            .Include(i => i.Category)
            //.Include(i => i.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incident>> GetByUserAsync(string userId)
    {
        return await _context.Incidents
            .Where(i => i.UserId == userId)
            .Include(i => i.Category)
            .ToListAsync();
    }

    public async Task<Incident?> GetIncidentWithDetailsAsync(int id)
    {
        return await _context.Incidents
            .Include(i => i.Category)
            //.Include(i => i.User)
            .Include(i => i.Comments)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<bool> AddAsync(Incident incident)
    {
        if (incident == null)
            return false;

        await _context.Incidents.AddAsync(incident);
        return true;
    }

    public Task<bool> UpdateAsync(Incident incident)
    {
        if (incident == null)
            return Task.FromResult(false);

        _context.Incidents.Update(incident);
        return Task.FromResult(true);
    }

    public Task<bool> RemoveAsync(Incident incident)
    {
        if (incident == null)
            return Task.FromResult(false);

        _context.Incidents.Remove(incident);
        return Task.FromResult(true);
    }
}
