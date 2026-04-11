using CadeiaAjuda.ApiService.Application.DTOs;
using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Repositories;

namespace CadeiaAjuda.ApiService.Application.Services;

public class ReasonService : IReasonService
{
    private readonly IReasonRepository _repository;

    public ReasonService(IReasonRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReasonDto>> GetAllAsync()
    {
        var reasons = await _repository.GetAllWithIncludesAsync();
        return reasons.Select(MapToDto);
    }

    public async Task<ReasonDto?> GetByIdAsync(Guid id)
    {
        var reason = await _repository.GetByIdWithIncludesAsync(id);
        return reason is null ? null : MapToDto(reason);
    }

    public async Task<ReasonDto> CreateAsync(ReasonCreateDto dto)
    {
        if (await _repository.ExistsByNameAsync(dto.TenantId, dto.Name))
            throw new InvalidOperationException("Já existe um motivo com este nome nesta empresa.");

        var reason = new Reason
        {
            Name = dto.Name,
            Description = dto.Description,
            TenantId = dto.TenantId
        };

        await _repository.AddAsync(reason);
        await _repository.SaveChangesAsync();

        var created = await _repository.GetByIdWithIncludesAsync(reason.Id);
        return MapToDto(created!);
    }

    public async Task<ReasonDto?> UpdateAsync(ReasonUpdateDto dto)
    {
        var reason = await _repository.GetByIdWithIncludesAsync(dto.Id);
        if (reason is null) return null;

        if (await _repository.ExistsByNameAsync(dto.TenantId, dto.Name, dto.Id))
            throw new InvalidOperationException("Já existe um motivo com este nome nesta empresa.");

        reason.Name = dto.Name;
        reason.Description = dto.Description;
        reason.TenantId = dto.TenantId;
        reason.Active = dto.Active;

        _repository.Update(reason);
        await _repository.SaveChangesAsync();

        var updated = await _repository.GetByIdWithIncludesAsync(reason.Id);
        return MapToDto(updated!);
    }

    public async Task<bool> ToggleActiveAsync(Guid id)
    {
        var reason = await _repository.GetByIdAsync(id);
        if (reason is null) return false;

        reason.Active = !reason.Active;
        _repository.Update(reason);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static ReasonDto MapToDto(Reason reason) => new()
    {
        Id = reason.Id,
        Name = reason.Name,
        Description = reason.Description,
        TenantId = reason.TenantId,
        TenantName = reason.Tenant?.Name ?? string.Empty,
        Active = reason.Active,
        CreatedAt = reason.CreatedAt
    };
}
