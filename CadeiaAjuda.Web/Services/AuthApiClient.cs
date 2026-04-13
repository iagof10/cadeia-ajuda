using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class AuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResult> LoginAsync(string tenantIdentifier, string login, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new
        {
            TenantIdentifier = tenantIdentifier,
            Login = login,
            Password = password
        });

        if (!response.IsSuccessStatusCode)
            return new AuthResult { Success = false };

        var user = await response.Content.ReadFromJsonAsync<LoggedUser>();
        return new AuthResult { Success = true, User = user };
    }
}

public class LoggedUser
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string TenantIdentifier { get; set; } = string.Empty;
    public Guid? RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = [];
}

public class AuthResult
{
    public bool Success { get; set; }
    public LoggedUser? User { get; set; }
}
