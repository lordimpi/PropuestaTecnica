using Mapster;
using PropuestaTecnica.Bussiness.Services.Contracts;
using PropuestaTecnica.Common.DTOs.Comments;
using PropuestaTecnica.Common.Entities;
using PropuestaTecnica.DataAccess.UnitOfWork;

namespace PropuestaTecnica.Bussiness.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CommentResponseDTO?> AddCommentAsync(int incidentId, CommentCreateDTO dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var incident = await _unitOfWork.IncidentsRepository.GetByIdAsync(incidentId);
            if (incident == null)
            {
                await _unitOfWork.RollbackAsync();
                return null;
            }

            var comment = dto.Adapt<Comment>();
            comment.IncidentId = incidentId;

            var added = await _unitOfWork.CommentsRepository.AddAsync(comment);
            if (!added)
            {
                await _unitOfWork.RollbackAsync();
                return null;
            }

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
            {
                await _unitOfWork.RollbackAsync();
                return null;
            }

            await _unitOfWork.CommitAsync();

            return comment.Adapt<CommentResponseDTO>();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<CommentResponseDTO>> GetByIncidentAsync(int incidentId)
    {
        var comments = await _unitOfWork.CommentsRepository.GetByIncidentIdAsync(incidentId);
        return comments.Adapt<IEnumerable<CommentResponseDTO>>();
    }
}