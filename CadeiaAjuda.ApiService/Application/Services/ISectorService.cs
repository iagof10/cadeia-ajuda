using CadeiaAjuda.ApiService.Application.DTOs;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface ISectorService
{
    Task<IEnumerable<SectorDto>> GetAllAsync();
    Task<SectorDto?> GetByIdAsync(Guid id);
    Task<SectorDto> CreateAsync(SectorCreateDto dto);
    Task<SectorDto?> UpdateAsync(SectorUpdateDto dto);
    Task<bool> ToggleActiveAsync(Guid id);
}
