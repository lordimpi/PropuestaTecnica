using PropuestaTecnica.Common.Entities;
using PropuestaTecnica.Common.Enums;

namespace PropuestaTecnica.DataAccess.Repositories.Contracts;

public interface IIncidentRepository
{
    Task<IEnumerable<Incident>> GetAllAsync();
    Task<Incident?> GetByIdAsync(int id);
    Task<IEnumerable<Incident>> GetByStatusAsync(IncidentStatus status);
    Task<IEnumerable<Incident>> GetByUserAsync(string userId);
    Task<Incident?> GetIncidentWithDetailsAsync(int id);
    Task<bool> AddAsync(Incident incident);
    Task<bool> UpdateAsync(Incident incident);
    Task<bool> RemoveAsync(Incident incident);

    IQueryable<Incident> GetQueryable();
}
