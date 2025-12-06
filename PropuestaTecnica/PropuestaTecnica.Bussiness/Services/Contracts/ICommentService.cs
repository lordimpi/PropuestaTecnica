using PropuestaTecnica.Common.DTOs.Comments;

namespace PropuestaTecnica.Bussiness.Services.Contracts;

public interface ICommentService
{
    Task<CommentResponseDTO?> AddCommentAsync(int incidentId, CommentCreateDTO dto);
    Task<IEnumerable<CommentResponseDTO>> GetByIncidentAsync(int incidentId);
}
