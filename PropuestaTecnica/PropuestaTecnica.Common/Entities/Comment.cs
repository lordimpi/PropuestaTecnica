namespace PropuestaTecnica.Common.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; } = null!;
    public int IncidentId { get; set; }
    public Incident Incident { get; set; } = null!;
}
