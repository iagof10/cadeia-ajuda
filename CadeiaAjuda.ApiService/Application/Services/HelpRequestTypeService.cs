using CadeiaAjuda.ApiService.Application.DTOs;
using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Repositories;

namespace CadeiaAjuda.ApiService.Application.Services;

public class HelpRequestTypeService : IHelpRequestTypeService
{
    private readonly IHelpRequestTypeRepository _repository;

    public HelpRequestTypeService(IHelpRequestTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HelpRequestTypeDto>> GetAllAsync()
    {
        var items = await _repository.GetAllWithIncludesAsync();
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<HelpRequestTypeDto>> GetByTenantIdAsync(Guid tenantId)
    {
        var items = await _repository.GetByTenantIdAsync(tenantId);
        return items.Select(MapToDto);
    }

    public async Task<HelpRequestTypeDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdWithIncludesAsync(id);
        return item is null ? null : MapToDto(item);
    }

    public async Task<HelpRequestTypeDto> CreateAsync(HelpRequestTypeCreateDto dto)
    {
        if (await _repository.ExistsByNameAsync(dto.TenantId, dto.Name))
            throw new InvalidOperationException("Já existe um tipo de solicitação com este nome nesta empresa.");

        var item = new HelpRequestType
        {
            Name = dto.Name,
            Description = dto.Description,
            SectorId = dto.SectorId,
            TenantId = dto.TenantId
        };

        await _repository.AddAsync(item);
        await _repository.SaveChangesAsync();

        var created = await _repository.GetByIdWithIncludesAsync(item.Id);
        return MapToDto(created!);
    }

    public async Task<HelpRequestTypeDto?> UpdateAsync(HelpRequestTypeUpdateDto dto)
    {
        var item = await _repository.GetByIdWithIncludesAsync(dto.Id);
        if (item is null) return null;

        if (await _repository.ExistsByNameAsync(dto.TenantId, dto.Name, dto.Id))
            throw new InvalidOperationException("Já existe um tipo de solicitação com este nome nesta empresa.");

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.SectorId = dto.SectorId;
        item.TenantId = dto.TenantId;
        item.Active = dto.Active;

        _repository.Update(item);
        await _repository.SaveChangesAsync();

        var updated = await _repository.GetByIdWithIncludesAsync(item.Id);
        return MapToDto(updated!);
    }

    public async Task<bool> ToggleActiveAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null) return false;

        item.Active = !item.Active;
        _repository.Update(item);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static HelpRequestTypeDto MapToDto(HelpRequestType item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        SectorId = item.SectorId,
        SectorName = item.Sector?.Name ?? string.Empty,
        SectorColor = item.Sector?.Color ?? string.Empty,
        TenantId = item.TenantId,
        TenantName = item.Tenant?.Name ?? string.Empty,
        Active = item.Active,
        CreatedAt = item.CreatedAt
    };
}
