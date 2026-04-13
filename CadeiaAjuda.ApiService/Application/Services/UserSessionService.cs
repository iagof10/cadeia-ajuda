using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Repositories;
using System.Security.Cryptography;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface IUserSessionService
{
    Task<UserSessionDto> CreateSessionAsync(Guid userId, Guid tenantId, string? ipAddress, string? userAgent);
    Task<UserSessionDto?> ValidateSessionAsync(string sessionToken);
    Task InvalidateSessionAsync(string sessionToken);
    Task InvalidateAllUserSessionsAsync(Guid userId);
    Task UpdateActivityAsync(string sessionToken);
}

public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _repository;

    public UserSessionService(IUserSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserSessionDto> CreateSessionAsync(Guid userId, Guid tenantId, string? ipAddress, string? userAgent)
    {
        // Invalidate all previous active sessions for this user
        var activeSessions = await _repository.GetActiveByUserIdAsync(userId);
        foreach (var s in activeSessions)
        {
            s.IsActive = false;
            s.LogoutAt = DateTime.UtcNow;
            _repository.Update(s);
        }

        var session = new UserSession
        {
            UserId = userId,
            TenantId = tenantId,
            SessionToken = GenerateToken(),
            LoginAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow,
            IsActive = true,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        await _repository.AddAsync(session);
        await _repository.SaveChangesAsync();

        return MapToDto(session);
    }

    public async Task<UserSessionDto?> ValidateSessionAsync(string sessionToken)
    {
        var session = await _repository.GetByTokenAsync(sessionToken);
        if (session is null || !session.IsActive)
            return null;

        return MapToDto(session);
    }

    public async Task InvalidateSessionAsync(string sessionToken)
    {
        var session = await _repository.GetByTokenAsync(sessionToken);
        if (session is null) return;

        session.IsActive = false;
        session.LogoutAt = DateTime.UtcNow;
        _repository.Update(session);
        await _repository.SaveChangesAsync();
    }

    public async Task InvalidateAllUserSessionsAsync(Guid userId)
    {
        var sessions = await _repository.GetActiveByUserIdAsync(userId);
        foreach (var s in sessions)
        {
            s.IsActive = false;
            s.LogoutAt = DateTime.UtcNow;
            _repository.Update(s);
        }
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateActivityAsync(string sessionToken)
    {
        var session = await _repository.GetByTokenAsync(sessionToken);
        if (session is null || !session.IsActive) return;

        session.LastActivityAt = DateTime.UtcNow;
        _repository.Update(session);
        await _repository.SaveChangesAsync();
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    private static UserSessionDto MapToDto(UserSession s) => new()
    {
        Id = s.Id,
        UserId = s.UserId,
        TenantId = s.TenantId,
        SessionToken = s.SessionToken,
        LoginAt = s.LoginAt,
        LastActivityAt = s.LastActivityAt,
        LogoutAt = s.LogoutAt,
        IsActive = s.IsActive,
        UserName = s.User?.Name ?? string.Empty
    };
}

public class UserSessionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string SessionToken { get; set; } = string.Empty;
    public DateTime LoginAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public DateTime? LogoutAt { get; set; }
    public bool IsActive { get; set; }
    public string UserName { get; set; } = string.Empty;
}
