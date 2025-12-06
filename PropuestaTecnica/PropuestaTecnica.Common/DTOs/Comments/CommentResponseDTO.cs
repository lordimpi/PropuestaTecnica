namespace PropuestaTecnica.Common.DTOs.Comments;

public class CommentResponseDTO
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UserId { get; set; } = string.Empty;
    public string? UserEmail { get; set; } = string.Empty;
}
