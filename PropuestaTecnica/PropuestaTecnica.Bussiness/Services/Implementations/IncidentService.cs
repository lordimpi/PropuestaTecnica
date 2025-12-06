using Mapster;
using PropuestaTecnica.Bussiness.Services.Contracts;
using PropuestaTecnica.Common.DTOs.Incidents;
using PropuestaTecnica.Common.Entities;
using PropuestaTecnica.DataAccess.UnitOfWork;

namespace PropuestaTecnica.Bussiness.Services.Implementations;

public class IncidentService : IIncidentService
{
    private readonly IUnitOfWork _unitOfWork;

    public IncidentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IncidentResponseDTO?> CreateAsync(IncidentCreateDTO dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var category = await _unitOfWork.CategoriesRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                await _unitOfWork.RollbackAsync();
                return null;
            }

            var entity = dto.Adapt<Incident>();
            entity.Status = Common.Enums.IncidentStatus.Open;

            var added = await _unitOfWork.IncidentsRepository.AddAsync(entity);
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

            var created = await _unitOfWork.IncidentsRepository.GetIncidentWithDetailsAsync(entity.Id);
            return created?.Adapt<IncidentResponseDTO>();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<IncidentResponseDTO>> GetAllAsync()
    {
        var list = await _unitOfWork.IncidentsRepository.GetAllAsync();
        return list.Adapt<IEnumerable<IncidentResponseDTO>>();
    }

    public async Task<IncidentResponseDTO?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.IncidentsRepository.GetIncidentWithDetailsAsync(id);
        return entity?.Adapt<IncidentResponseDTO>();
    }

    public async Task<bool> UpdateAsync(int id, IncidentUpdateDTO dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var entity = await _unitOfWork.IncidentsRepository.GetByIdAsync(id);
            if (entity == null)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }

            dto.Adapt(entity);

            var updated = await _unitOfWork.IncidentsRepository.UpdateAsync(entity);
            if (!updated)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }

            await _unitOfWork.CommitAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var entity = await _unitOfWork.IncidentsRepository.GetByIdAsync(id);
            if (entity == null)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }

            var removed = await _unitOfWork.IncidentsRepository.RemoveAsync(entity);
            if (!removed)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }

            await _unitOfWork.CommitAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
