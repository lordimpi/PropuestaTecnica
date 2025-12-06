using PropuestaTecnica.Common.Enums;

namespace PropuestaTecnica.Common.DTOs.Incidents;

public class IncidentResponseDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; }
    public string? UserId { get; set; } = string.Empty;
    public string? UserEmail { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}
