using CadeiaAjuda.ApiService.Application.DTOs;
using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Repositories;

namespace CadeiaAjuda.ApiService.Application.Services;

public class SectorService : ISectorService
{
    private readonly ISectorRepository _repository;

    public SectorService(ISectorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SectorDto>> GetAllAsync()
    {
        var sectors = await _repository.GetAllWithIncludesAsync();
        return sectors.Select(MapToDto);
    }

    public async Task<IEnumerable<SectorDto>> GetByTenantIdAsync(Guid tenantId)
    {
        var sectors = await _repository.GetByTenantIdAsync(tenantId);
        return sectors.Select(MapToDto);
    }

    public async Task<SectorDto?> GetByIdAsync(Guid id)
    {
        var sector = await _repository.GetByIdWithIncludesAsync(id);
        return sector is null ? null : MapToDto(sector);
    }

    public async Task<SectorDto> CreateAsync(SectorCreateDto dto)
    {
        if (await _repository.ExistsByNameAsync(dto.TenantId, dto.Name))
            throw new InvalidOperationException("Já existe um setor com este nome nesta empresa.");

        var sector = new Sector
        {
            Name = dto.Name,
            Color = dto.Color,
            TenantId = dto.TenantId
        };

        await _repository.AddAsync(sector);
        await _repository.SaveChangesAsync();

        var created = await _repository.GetByIdWithIncludesAsync(sector.Id);
        return MapToDto(created!);
    }

    public async Task<SectorDto?> UpdateAsync(SectorUpdateDto dto)
    {
        var sector = await _repository.GetByIdWithIncludesAsync(dto.Id);
        if (sector is null) return null;

        if (await _repository.ExistsByNameAsync(dto.TenantId, dto.Name, dto.Id))
            throw new InvalidOperationException("Já existe um setor com este nome nesta empresa.");

        sector.Name = dto.Name;
        sector.Color = dto.Color;
        sector.TenantId = dto.TenantId;
        sector.Active = dto.Active;

        _repository.Update(sector);
        await _repository.SaveChangesAsync();

        var updated = await _repository.GetByIdWithIncludesAsync(sector.Id);
        return MapToDto(updated!);
    }

    public async Task<bool> ToggleActiveAsync(Guid id)
    {
        var sector = await _repository.GetByIdAsync(id);
        if (sector is null) return false;

        sector.Active = !sector.Active;
        _repository.Update(sector);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static SectorDto MapToDto(Sector sector) => new()
    {
        Id = sector.Id,
        Name = sector.Name,
        Description = sector.Description,
        Color = sector.Color,
        TenantId = sector.TenantId,
        TenantName = sector.Tenant?.Name ?? string.Empty,
        Active = sector.Active,
        CreatedAt = sector.CreatedAt
    };
}
