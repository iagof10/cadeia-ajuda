using CadeiaAjuda.ApiService.Application.DTOs;
using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Repositories;

namespace CadeiaAjuda.ApiService.Application.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _repository;

    public TenantService(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TenantDto>> GetAllAsync()
    {
        var tenants = await _repository.GetAllAsync();
        return tenants.Select(MapToDto);
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id)
    {
        var tenant = await _repository.GetByIdAsync(id);
        return tenant is null ? null : MapToDto(tenant);
    }

    public async Task<TenantDto> CreateAsync(TenantCreateDto dto)
    {
        if (await _repository.ExistsByCnpjAsync(dto.Cnpj))
            throw new InvalidOperationException("Já existe um tenant com este CNPJ.");

        if (await _repository.ExistsByIdentifierAsync(dto.Identifier))
            throw new InvalidOperationException("Já existe um tenant com este identificador.");

        var tenant = new Tenant
        {
            Name = dto.Name,
            TradeName = dto.TradeName,
            Cnpj = dto.Cnpj,
            Email = dto.Email,
            Phone = dto.Phone,
            Identifier = dto.Identifier,
            StandardUserLimit = dto.StandardUserLimit,
            AndonUserLimit = dto.AndonUserLimit,
            ManagerUserLimit = dto.ManagerUserLimit,
            AdministratorUserLimit = dto.AdministratorUserLimit
        };

        await _repository.AddAsync(tenant);
        await _repository.SaveChangesAsync();

        return MapToDto(tenant);
    }

    public async Task<TenantDto?> UpdateAsync(TenantUpdateDto dto)
    {
        var tenant = await _repository.GetByIdAsync(dto.Id);
        if (tenant is null) return null;

        if (await _repository.ExistsByCnpjAsync(dto.Cnpj, dto.Id))
            throw new InvalidOperationException("Já existe um tenant com este CNPJ.");

        if (await _repository.ExistsByIdentifierAsync(dto.Identifier, dto.Id))
            throw new InvalidOperationException("Já existe um tenant com este identificador.");

        tenant.Name = dto.Name;
        tenant.TradeName = dto.TradeName;
        tenant.Cnpj = dto.Cnpj;
        tenant.Email = dto.Email;
        tenant.Phone = dto.Phone;
        tenant.Identifier = dto.Identifier;
        tenant.Active = dto.Active;
        tenant.StandardUserLimit = dto.StandardUserLimit;
        tenant.AndonUserLimit = dto.AndonUserLimit;
        tenant.ManagerUserLimit = dto.ManagerUserLimit;
        tenant.AdministratorUserLimit = dto.AdministratorUserLimit;

        _repository.Update(tenant);
        await _repository.SaveChangesAsync();

        return MapToDto(tenant);
    }

    public async Task<bool> ToggleActiveAsync(Guid id)
    {
        var tenant = await _repository.GetByIdAsync(id);
        if (tenant is null) return false;

        tenant.Active = !tenant.Active;
        _repository.Update(tenant);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static TenantDto MapToDto(Tenant tenant) => new()
    {
        Id = tenant.Id,
        Name = tenant.Name,
        TradeName = tenant.TradeName,
        Cnpj = tenant.Cnpj,
        Email = tenant.Email,
        Phone = tenant.Phone,
        Identifier = tenant.Identifier,
        Active = tenant.Active,
        CreatedAt = tenant.CreatedAt,
        StandardUserLimit = tenant.StandardUserLimit,
        AndonUserLimit = tenant.AndonUserLimit,
        ManagerUserLimit = tenant.ManagerUserLimit,
        AdministratorUserLimit = tenant.AdministratorUserLimit
    };
}