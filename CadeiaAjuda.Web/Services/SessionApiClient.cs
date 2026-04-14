using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class SessionApiClient
{
    private readonly HttpClient _http;

    public SessionApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<SessionResult?> CreateSessionAsync(Guid userId, Guid tenantId, string? ipAddress, string? userAgent)
    {
        var response = await _http.PostAsJsonAsync("/api/sessions", new
        {
            UserId = userId,
            TenantId = tenantId,
            IpAddress = ipAddress,
            UserAgent = userAgent
        });

        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<SessionResult>();
    }

    public async Task<SessionResult?> ValidateSessionAsync(string sessionToken)
    {
        try
        {
            return await _http.GetFromJsonAsync<SessionResult>($"/api/sessions/validate/{sessionToken}");
        }
        catch
        {
            return null;
        }
    }

    public async Task InvalidateSessionAsync(string sessionToken)
    {
        await _http.PostAsync($"/api/sessions/invalidate/{sessionToken}", null);
    }

    public async Task UpdateActivityAsync(string sessionToken)
    {
        await _http.PatchAsync($"/api/sessions/activity/{sessionToken}", null);
    }

    public async Task<bool> HasActiveSessionAsync(Guid userId)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<ActiveSessionResult>($"/api/sessions/active/{userId}");
            return result?.HasActiveSession ?? false;
        }
        catch
        {
            return false;
        }
    }
}

public class ActiveSessionResult
{
    public bool HasActiveSession { get; set; }
}

public class SessionResult
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
