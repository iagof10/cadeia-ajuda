using CadeiaAjuda.ApiService.Application.DTOs;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface IHelpRequestService
{
    Task<IEnumerable<HelpRequestDto>> GetAllAsync();
    Task<IEnumerable<HelpRequestDto>> GetByTenantIdAsync(Guid tenantId);
    Task<HelpRequestDto?> GetByIdAsync(Guid id);
    Task<HelpRequestDto> CreateAsync(HelpRequestCreateDto dto);
    Task<HelpRequestDto?> CloseAsync(Guid id, HelpRequestCloseDto dto);
}
