using CadeiaAjuda.ApiService.Application.DTOs;
using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Repositories;

namespace CadeiaAjuda.ApiService.Application.Services;

public class EscalationLevelService : IEscalationLevelService
{
    private readonly IEscalationLevelRepository _repository;

    public EscalationLevelService(IEscalationLevelRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EscalationLevelDto>> GetAllAsync()
    {
        var levels = await _repository.GetAllWithIncludesAsync();
        return levels.Select(MapToDto);
    }

    public async Task<IEnumerable<EscalationLevelDto>> GetBySectorIdAsync(Guid sectorId)
    {
        var levels = await _repository.GetBySectorIdAsync(sectorId);
        return levels.Select(MapToDto);
    }

    public async Task<EscalationLevelDto?> GetByIdAsync(Guid id)
    {
        var level = await _repository.GetByIdWithIncludesAsync(id);
        return level is null ? null : MapToDto(level);
    }

    public async Task<EscalationLevelDto> CreateAsync(EscalationLevelCreateDto dto)
    {
        if (await _repository.ExistsByOrderAsync(dto.TenantId, dto.SectorId, dto.Order))
            throw new InvalidOperationException("Já existe um nível de escalonamento com esta ordem para este setor.");

        var level = new EscalationLevel
        {
            Name = dto.Name,
            Description = dto.Description,
            Order = dto.Order,
            EscalationTimeMinutes = dto.EscalationTimeMinutes,
            SectorId = dto.SectorId,
            TenantId = dto.TenantId,
            Responsibles = dto.Responsibles.Select(r => new EscalationLevelResponsible
            {
                UserId = r.UserId,
                IsPrimary = r.IsPrimary
            }).ToList()
        };

        await _repository.AddAsync(level);
        await _repository.SaveChangesAsync();

        var created = await _repository.GetByIdWithIncludesAsync(level.Id);
        return MapToDto(created!);
    }

    public async Task<EscalationLevelDto?> UpdateAsync(EscalationLevelUpdateDto dto)
    {
        var level = await _repository.GetByIdWithIncludesAsync(dto.Id);
        if (level is null) return null;

        if (await _repository.ExistsByOrderAsync(dto.TenantId, dto.SectorId, dto.Order, dto.Id))
            throw new InvalidOperationException("Já existe um nível de escalonamento com esta ordem para este setor.");

        level.Name = dto.Name;
        level.Description = dto.Description;
        level.Order = dto.Order;
        level.EscalationTimeMinutes = dto.EscalationTimeMinutes;
        level.SectorId = dto.SectorId;
        level.TenantId = dto.TenantId;
        level.Active = dto.Active;

        _repository.RemoveResponsibles(level.Responsibles);
        level.Responsibles = dto.Responsibles.Select(r => new EscalationLevelResponsible
        {
            UserId = r.UserId,
            IsPrimary = r.IsPrimary,
            EscalationLevelId = level.Id
        }).ToList();

        _repository.Update(level);
        await _repository.SaveChangesAsync();

        var updated = await _repository.GetByIdWithIncludesAsync(level.Id);
        return MapToDto(updated!);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var level = await _repository.GetByIdWithIncludesAsync(id);
        if (level is null) return false;

        _repository.RemoveResponsibles(level.Responsibles);
        level.Active = false;
        _repository.Update(level);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static EscalationLevelDto MapToDto(EscalationLevel level) => new()
    {
        Id = level.Id,
        Name = level.Name,
        Description = level.Description,
        Order = level.Order,
        EscalationTimeMinutes = level.EscalationTimeMinutes,
        SectorId = level.SectorId,
        SectorName = level.Sector?.Name ?? string.Empty,
        TenantId = level.TenantId,
        TenantName = level.Tenant?.Name ?? string.Empty,
        Active = level.Active,
        CreatedAt = level.CreatedAt,
        Responsibles = level.Responsibles.Select(r => new EscalationLevelResponsibleDto
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = r.User?.Name ?? string.Empty,
            IsPrimary = r.IsPrimary
        }).ToList()
    };
}
