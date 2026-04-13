using CadeiaAjuda.ApiService.Application.DTOs;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface IReasonService
{
    Task<IEnumerable<ReasonDto>> GetAllAsync();
    Task<ReasonDto?> GetByIdAsync(Guid id);
    Task<ReasonDto> CreateAsync(ReasonCreateDto dto);
    Task<ReasonDto?> UpdateAsync(ReasonUpdateDto dto);
    Task<bool> ToggleActiveAsync(Guid id);
}
