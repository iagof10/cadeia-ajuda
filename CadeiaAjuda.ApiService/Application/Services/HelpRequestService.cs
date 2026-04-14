using CadeiaAjuda.ApiService.Application.DTOs;
using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Repositories;

namespace CadeiaAjuda.ApiService.Application.Services;

public class HelpRequestService : IHelpRequestService
{
    private readonly IHelpRequestRepository _repository;

    public HelpRequestService(IHelpRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HelpRequestDto>> GetAllAsync()
    {
        var items = await _repository.GetAllWithIncludesAsync();
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<HelpRequestDto>> GetByTenantIdAsync(Guid tenantId)
    {
        var items = await _repository.GetByTenantIdAsync(tenantId);
        return items.Select(MapToDto);
    }

    public async Task<HelpRequestDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdWithIncludesAsync(id);
        return item is null ? null : MapToDto(item);
    }

    public async Task<HelpRequestDto> CreateAsync(HelpRequestCreateDto dto)
    {
        var hasDuplicate = await _repository.HasOpenBySectorAndAreaAsync(dto.TenantId, dto.SectorId, dto.AreaId);
        if (hasDuplicate)
            throw new InvalidOperationException("Já existe um chamado aberto para este setor e recurso.");

        var count = await _repository.CountByTenantAsync(dto.TenantId);
        var code = $"CA-{(count + 1):D5}";

        var entity = new HelpRequest
        {
            Code = code,
            Description = dto.Description,
            SectorId = dto.SectorId,
            HelpRequestTypeId = dto.HelpRequestTypeId,
            AreaId = dto.AreaId,
            RequestedByUserId = dto.RequestedByUserId,
            TenantId = dto.TenantId,
            Status = HelpRequestStatus.Open
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var created = await _repository.GetByIdWithIncludesAsync(entity.Id);
        return MapToDto(created!);
    }

    public async Task<HelpRequestDto?> CloseAsync(Guid id, HelpRequestCloseDto dto)
    {
        var entity = await _repository.GetByIdWithIncludesAsync(id);
        if (entity is null) return null;

        if (entity.Status == HelpRequestStatus.Closed)
            throw new InvalidOperationException("Este chamado já está encerrado.");

        entity.Status = HelpRequestStatus.Closed;
        entity.ClosedAt = DateTime.UtcNow;
        entity.ClosedByUserId = dto.ClosedByUserId;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        var updated = await _repository.GetByIdWithIncludesAsync(id);
        return MapToDto(updated!);
    }

    private static string StatusName(HelpRequestStatus status) => status switch
    {
        HelpRequestStatus.Open => "Aberto",
        HelpRequestStatus.InProgress => "Em Andamento",
        HelpRequestStatus.Escalated => "Escalonado",
        HelpRequestStatus.Resolved => "Resolvido",
        HelpRequestStatus.Closed => "Fechado",
        _ => "Desconhecido"
    };

    private static HelpRequestDto MapToDto(HelpRequest h) => new()
    {
        Id = h.Id,
        Code = h.Code,
        Description = h.Description,
        SectorId = h.SectorId,
        SectorName = h.Sector?.Name ?? string.Empty,
        SectorColor = h.Sector?.Color ?? string.Empty,
        HelpRequestTypeId = h.HelpRequestTypeId,
        HelpRequestTypeName = h.HelpRequestType?.Name ?? string.Empty,
        AreaId = h.AreaId,
        AreaName = h.Area?.Name ?? string.Empty,
        RequestedByUserId = h.RequestedByUserId,
        RequestedByUserName = h.RequestedByUser?.Name ?? string.Empty,
        ClosedByUserId = h.ClosedByUserId,
        ClosedByUserName = h.ClosedByUser?.Name ?? string.Empty,
        Status = (int)h.Status,
        StatusName = StatusName(h.Status),
        TenantId = h.TenantId,
        CreatedAt = h.CreatedAt,
        ClosedAt = h.ClosedAt,
        Active = h.Active
    };
}
