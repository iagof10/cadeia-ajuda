using CadeiaAjuda.ApiService.Application.DTOs;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface IAreaService
{
    Task<IEnumerable<AreaDto>> GetAllAsync();
    Task<IEnumerable<AreaDto>> GetByTenantIdAsync(Guid tenantId);
    Task<AreaDto?> GetByIdAsync(Guid id);
    Task<AreaDto> CreateAsync(AreaCreateDto dto);
    Task<AreaDto?> UpdateAsync(AreaUpdateDto dto);
    Task<bool> ToggleActiveAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}
