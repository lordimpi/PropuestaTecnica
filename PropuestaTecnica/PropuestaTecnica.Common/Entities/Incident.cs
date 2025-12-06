using PropuestaTecnica.Common.Enums;

namespace PropuestaTecnica.Common.Entities;

public class Incident
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public string? UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = [];
}
