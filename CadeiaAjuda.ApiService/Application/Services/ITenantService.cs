using CadeiaAjuda.ApiService.Application.DTOs;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface ITenantService
{
    Task<IEnumerable<TenantDto>> GetAllAsync();
    Task<TenantDto?> GetByIdAsync(Guid id);
    Task<TenantDto> CreateAsync(TenantCreateDto dto);
    Task<TenantDto?> UpdateAsync(TenantUpdateDto dto);
    Task<bool> ToggleActiveAsync(Guid id);
}