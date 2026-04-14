using System.Security.Cryptography;
using CadeiaAjuda.ApiService.Application.DTOs;
using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CadeiaAjuda.ApiService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ITenantRepository _tenantRepository;

    public UserService(IUserRepository repository, ITenantRepository tenantRepository)
    {
        _repository = repository;
        _tenantRepository = tenantRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _repository.GetAllWithIncludesAsync();
        return users.Select(MapToDto);
    }

    public async Task<IEnumerable<UserDto>> GetByTenantIdAsync(Guid tenantId)
    {
        var users = await _repository.GetByTenantIdAsync(tenantId);
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _repository.GetByIdWithIncludesAsync(id);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(UserCreateDto dto)
    {
        if (await _repository.ExistsByLoginAsync(dto.TenantId, dto.Login))
            throw new InvalidOperationException("Já existe um usuário com este login nesta empresa.");

        if (await _repository.ExistsByEmailAsync(dto.TenantId, dto.Email))
            throw new InvalidOperationException("Já existe um usuário com este e-mail nesta empresa.");

        await ValidateUserTypeLimitAsync(dto.TenantId, dto.UserType);

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Login = dto.Login,
            PasswordHash = HashPassword(dto.Password),
            TenantId = dto.TenantId,
            RoleId = dto.RoleId,
            UserType = dto.UserType
        };

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        var created = await _repository.GetByIdWithIncludesAsync(user.Id);
        return MapToDto(created!);
    }

    public async Task<UserDto?> UpdateAsync(UserUpdateDto dto)
    {
        var user = await _repository.GetByIdWithIncludesAsync(dto.Id);
        if (user is null) return null;

        if (await _repository.ExistsByLoginAsync(dto.TenantId, dto.Login, dto.Id))
            throw new InvalidOperationException("Já existe um usuário com este login nesta empresa.");

        if (await _repository.ExistsByEmailAsync(dto.TenantId, dto.Email, dto.Id))
            throw new InvalidOperationException("Já existe um usuário com este e-mail nesta empresa.");

        await ValidateUserTypeLimitAsync(dto.TenantId, dto.UserType, dto.Id);

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.Phone = dto.Phone;
        user.Login = dto.Login;
        user.TenantId = dto.TenantId;
        user.Active = dto.Active;
        user.RoleId = dto.RoleId;
        user.UserType = dto.UserType;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.PasswordHash = HashPassword(dto.Password);
        }

        _repository.Update(user);
        await _repository.SaveChangesAsync();

        var updated = await _repository.GetByIdWithIncludesAsync(user.Id);
        return MapToDto(updated!);
    }

    public async Task<bool> ToggleActiveAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user is null) return false;

        user.Active = !user.Active;
        _repository.Update(user);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<UserDto?> AuthenticateAsync(LoginDto dto)
    {
        var tenant = await _tenantRepository.GetByIdentifierAsync(dto.TenantIdentifier);
        if (tenant is null || !tenant.Active) return null;

        var user = await _repository.GetByLoginAsync(tenant.Id, dto.Login);
        if (user is null) return null;

        if (!VerifyPassword(dto.Password, user.PasswordHash)) return null;

        return MapToDto(user);
    }

    private async Task ValidateUserTypeLimitAsync(Guid tenantId, UserType userType, Guid? excludeUserId = null)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId)
            ?? throw new InvalidOperationException("Empresa não encontrada.");

        var limit = userType switch
        {
            UserType.Andon => tenant.AndonUserLimit,
            UserType.Manager => tenant.ManagerUserLimit,
            UserType.Administrator => tenant.AdministratorUserLimit,
            _ => tenant.StandardUserLimit
        };

        if (!limit.HasValue)
            return;

        var count = await _repository.CountByTenantAndTypeAsync(tenantId, userType, excludeUserId);
        if (count >= limit.Value)
            throw new InvalidOperationException($"Limite de usuários do tipo {GetUserTypeName(userType)} atingido para esta empresa.");
    }

    private static string GetUserTypeName(UserType userType) => userType switch
    {
        UserType.Andon => "Andon",
        UserType.Manager => "Manager",
        UserType.Administrator => "Administrator",
        _ => "Padrão"
    };

    private static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32));

        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32));

        return hashed == parts[1];
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Phone = user.Phone,
        Login = user.Login,
        TenantId = user.TenantId,
        TenantName = user.Tenant?.Name ?? string.Empty,
        TenantIdentifier = user.Tenant?.Identifier ?? string.Empty,
        Active = user.Active,
        CreatedAt = user.CreatedAt,
        RoleId = user.RoleId,
        RoleName = user.Role?.Name ?? string.Empty,
        UserType = user.UserType,
        UserTypeName = GetUserTypeName(user.UserType),
        Permissions = user.Role?.RolePermissions
            .Select(rp => rp.Permission.Key)
            .ToList() ?? []
    };
}
