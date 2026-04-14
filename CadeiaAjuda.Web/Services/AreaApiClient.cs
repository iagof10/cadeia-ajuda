using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class AreaApiClient
{
    private readonly HttpClient _http;

    public AreaApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<AreaViewModel>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<AreaViewModel>>("/api/areas") ?? [];

    public async Task<List<AreaViewModel>> GetByTenantIdAsync(Guid tenantId)
        => await _http.GetFromJsonAsync<List<AreaViewModel>>($"/api/areas/by-tenant/{tenantId}") ?? [];

    public async Task<AreaViewModel?> GetByIdAsync(Guid id)
        => await _http.GetFromJsonAsync<AreaViewModel>($"/api/areas/{id}");

    public async Task<HttpResponseMessage> CreateAsync(AreaFormModel model)
        => await _http.PostAsJsonAsync("/api/areas", new
        {
            model.Name,
            model.Description,
            model.ParentId,
            model.TenantId,
        });

    public async Task<HttpResponseMessage> UpdateAsync(Guid id, AreaFormModel model)
        => await _http.PutAsJsonAsync($"/api/areas/{id}", new
        {
            model.Name,
            model.Description,
            model.ParentId,
            model.TenantId,
            model.Active,
        });

    public async Task<HttpResponseMessage> ToggleActiveAsync(Guid id)
        => await _http.PatchAsync($"/api/areas/{id}/toggle-active", null);

    public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        => await _http.DeleteAsync($"/api/areas/{id}");
}

public class AreaViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Guid TenantId { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AreaFormModel
{
    [Required(ErrorMessage = "Informe o nome da área.")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }

    public Guid TenantId { get; set; }

    public bool Active { get; set; } = true;
}
