using CadeiaAjuda.ApiService.Application.DTOs;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface IEscalationLevelService
{
    Task<IEnumerable<EscalationLevelDto>> GetAllAsync();
    Task<IEnumerable<EscalationLevelDto>> GetBySectorIdAsync(Guid sectorId);
    Task<EscalationLevelDto?> GetByIdAsync(Guid id);
    Task<EscalationLevelDto> CreateAsync(EscalationLevelCreateDto dto);
    Task<EscalationLevelDto?> UpdateAsync(EscalationLevelUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
