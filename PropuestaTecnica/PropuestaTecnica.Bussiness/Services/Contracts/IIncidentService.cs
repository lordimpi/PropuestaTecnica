using PropuestaTecnica.Common.DTOs.Incidents;

namespace PropuestaTecnica.Bussiness.Services.Contracts;

public interface IIncidentService
{
    Task<IncidentResponseDTO?> CreateAsync(IncidentCreateDTO dto);
    Task<IEnumerable<IncidentResponseDTO>> GetAllAsync();
    Task<IncidentResponseDTO?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, IncidentUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}
