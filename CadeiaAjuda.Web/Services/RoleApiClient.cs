using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class RoleApiClient
{
    private readonly HttpClient _http;

    public RoleApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<RoleViewModel>> GetByTenantIdAsync(Guid tenantId)
        => await _http.GetFromJsonAsync<List<RoleViewModel>>($"/api/roles/by-tenant/{tenantId}") ?? [];

    public async Task<RoleViewModel?> GetByIdAsync(Guid id)
        => await _http.GetFromJsonAsync<RoleViewModel>($"/api/roles/{id}");

    public async Task<HttpResponseMessage> CreateAsync(RoleSaveModel model)
        => await _http.PostAsJsonAsync("/api/roles", model);

    public async Task<HttpResponseMessage> UpdateAsync(Guid id, RoleSaveModel model)
        => await _http.PutAsJsonAsync($"/api/roles/{id}", model);

    public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        => await _http.DeleteAsync($"/api/roles/{id}");

    public async Task<List<PermissionViewModel>> GetAllPermissionsAsync()
        => await _http.GetFromJsonAsync<List<PermissionViewModel>>("/api/permissions") ?? [];
}

public class RoleViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Permissions { get; set; } = [];
}

public class RoleSaveModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public bool Active { get; set; } = true;
    public List<string> Permissions { get; set; } = [];
}

public class PermissionViewModel
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
