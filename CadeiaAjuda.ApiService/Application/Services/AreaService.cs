using CadeiaAjuda.ApiService.Application.DTOs;
using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Repositories;

namespace CadeiaAjuda.ApiService.Application.Services;

public class AreaService : IAreaService
{
    private readonly IAreaRepository _repository;

    public AreaService(IAreaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AreaDto>> GetAllAsync()
    {
        var areas = await _repository.GetAllWithIncludesAsync();
        return areas.Select(MapToDto);
    }

    public async Task<IEnumerable<AreaDto>> GetByTenantIdAsync(Guid tenantId)
    {
        var areas = await _repository.GetByTenantIdAsync(tenantId);
        return areas.Select(MapToDto);
    }

    public async Task<AreaDto?> GetByIdAsync(Guid id)
    {
        var area = await _repository.GetByIdWithIncludesAsync(id);
        return area is null ? null : MapToDto(area);
    }

    public async Task<AreaDto> CreateAsync(AreaCreateDto dto)
    {
        if (await _repository.ExistsByNameAsync(dto.TenantId, dto.Name, dto.ParentId))
            throw new InvalidOperationException("Já existe uma área com este nome neste nível.");

        var area = new Area
        {
            Name = dto.Name,
            Description = dto.Description,
            ParentId = dto.ParentId,
            TenantId = dto.TenantId
        };

        await _repository.AddAsync(area);
        await _repository.SaveChangesAsync();

        var created = await _repository.GetByIdWithIncludesAsync(area.Id);
        return MapToDto(created!);
    }

    public async Task<AreaDto?> UpdateAsync(AreaUpdateDto dto)
    {
        var area = await _repository.GetByIdWithIncludesAsync(dto.Id);
        if (area is null) return null;

        if (await _repository.ExistsByNameAsync(dto.TenantId, dto.Name, dto.ParentId, dto.Id))
            throw new InvalidOperationException("Já existe uma área com este nome neste nível.");

        area.Name = dto.Name;
        area.Description = dto.Description;
        area.ParentId = dto.ParentId;
        area.TenantId = dto.TenantId;
        area.Active = dto.Active;

        _repository.Update(area);
        await _repository.SaveChangesAsync();

        var updated = await _repository.GetByIdWithIncludesAsync(area.Id);
        return MapToDto(updated!);
    }

    public async Task<bool> ToggleActiveAsync(Guid id)
    {
        var area = await _repository.GetByIdAsync(id);
        if (area is null) return false;

        area.Active = !area.Active;
        _repository.Update(area);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var area = await _repository.GetByIdAsync(id);
        if (area is null) return false;

        if (await _repository.HasChildrenAsync(id))
            throw new InvalidOperationException("Não é possível excluir uma área que possui sub-áreas. Remova as sub-áreas primeiro.");

        _repository.Remove(area);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static AreaDto MapToDto(Area area) => new()
    {
        Id = area.Id,
        Name = area.Name,
        Description = area.Description,
        ParentId = area.ParentId,
        TenantId = area.TenantId,
        TenantName = area.Tenant?.Name ?? string.Empty,
        Active = area.Active,
        CreatedAt = area.CreatedAt
    };
}
