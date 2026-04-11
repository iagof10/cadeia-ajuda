using CadeiaAjuda.ApiService.Application.DTOs;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface IHelpRequestTypeService
{
    Task<IEnumerable<HelpRequestTypeDto>> GetAllAsync();
    Task<HelpRequestTypeDto?> GetByIdAsync(Guid id);
    Task<HelpRequestTypeDto> CreateAsync(HelpRequestTypeCreateDto dto);
    Task<HelpRequestTypeDto?> UpdateAsync(HelpRequestTypeUpdateDto dto);
    Task<bool> ToggleActiveAsync(Guid id);
}
