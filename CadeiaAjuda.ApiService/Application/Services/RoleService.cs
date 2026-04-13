using CadeiaAjuda.ApiService.Application.DTOs;
using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetByTenantIdAsync(Guid tenantId);
    Task<RoleDto?> GetByIdAsync(Guid id);
    Task<RoleDto> CreateAsync(RoleCreateDto dto);
    Task<RoleDto?> UpdateAsync(RoleUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
}

public class RoleService : IRoleService
{
    private readonly AppDbContext _db;

    public RoleService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<RoleDto>> GetByTenantIdAsync(Guid tenantId)
    {
        var roles = await _db.Roles
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Where(r => r.TenantId == tenantId)
            .OrderBy(r => r.Name)
            .ToListAsync();

        return roles.Select(MapToDto);
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await _db.Roles
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);

        return role is null ? null : MapToDto(role);
    }

    public async Task<RoleDto> CreateAsync(RoleCreateDto dto)
    {
        var exists = await _db.Roles.AnyAsync(r => r.TenantId == dto.TenantId && r.Name == dto.Name);
        if (exists)
            throw new InvalidOperationException("Já existe um perfil com este nome nesta empresa.");

        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description,
            TenantId = dto.TenantId
        };

        _db.Roles.Add(role);
        await _db.SaveChangesAsync();

        // Add permissions
        await SetPermissionsAsync(role.Id, dto.Permissions);

        return (await GetByIdAsync(role.Id))!;
    }

    public async Task<RoleDto?> UpdateAsync(RoleUpdateDto dto)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == dto.Id);
        if (role is null) return null;

        var duplicateName = await _db.Roles.AnyAsync(r =>
            r.TenantId == dto.TenantId && r.Name == dto.Name && r.Id != dto.Id);
        if (duplicateName)
            throw new InvalidOperationException("Já existe um perfil com este nome nesta empresa.");

        role.Name = dto.Name;
        role.Description = dto.Description;
        role.Active = dto.Active;

        await _db.SaveChangesAsync();

        await SetPermissionsAsync(role.Id, dto.Permissions);

        return (await GetByIdAsync(role.Id))!;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role is null) return false;

        var hasUsers = await _db.Users.AnyAsync(u => u.RoleId == id);
        if (hasUsers)
            throw new InvalidOperationException("Este perfil possui usuários vinculados. Remova os vínculos antes de excluir.");

        _db.Roles.Remove(role);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
    {
        var permissions = await _db.Permissions
            .OrderBy(p => p.SortOrder)
            .ToListAsync();

        return permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Key = p.Key,
            Name = p.Name,
            Group = p.Group,
            SortOrder = p.SortOrder
        });
    }

    private async Task SetPermissionsAsync(Guid roleId, List<string> permissionKeys)
    {
        // Remove existing
        var existing = await _db.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
        _db.RolePermissions.RemoveRange(existing);

        // Add new
        if (permissionKeys.Count > 0)
        {
            var permissions = await _db.Permissions
                .Where(p => permissionKeys.Contains(p.Key))
                .ToListAsync();

            foreach (var perm in permissions)
            {
                _db.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = perm.Id
                });
            }
        }

        await _db.SaveChangesAsync();
    }

    private static RoleDto MapToDto(Role role) => new()
    {
        Id = role.Id,
        Name = role.Name,
        Description = role.Description,
        TenantId = role.TenantId,
        Active = role.Active,
        CreatedAt = role.CreatedAt,
        Permissions = role.RolePermissions.Select(rp => rp.Permission.Key).ToList()
    };
}
