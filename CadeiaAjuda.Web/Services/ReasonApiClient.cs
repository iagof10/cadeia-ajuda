using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class ReasonApiClient
{
    private readonly HttpClient _httpClient;

    public ReasonApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ReasonViewModel>> GetAllAsync()
        => await _httpClient.GetFromJsonAsync<List<ReasonViewModel>>("/api/reasons") ?? [];

    public async Task<List<ReasonViewModel>> GetByTenantIdAsync(Guid tenantId)
        => await _httpClient.GetFromJsonAsync<List<ReasonViewModel>>($"/api/reasons/by-tenant/{tenantId}") ?? [];

    public async Task<ReasonViewModel?> GetByIdAsync(Guid id)
        => await _httpClient.GetFromJsonAsync<ReasonViewModel>($"/api/reasons/{id}");

    public async Task<HttpResponseMessage> CreateAsync(ReasonFormModel model)
        => await _httpClient.PostAsJsonAsync("/api/reasons", model);

    public async Task<HttpResponseMessage> UpdateAsync(Guid id, ReasonFormModel model)
        => await _httpClient.PutAsJsonAsync($"/api/reasons/{id}", model);

    public async Task<HttpResponseMessage> ToggleActiveAsync(Guid id)
        => await _httpClient.PatchAsync($"/api/reasons/{id}/toggle-active", null);
}

public class ReasonViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReasonFormModel
{
    [Required(ErrorMessage = "Informe o nome do motivo.")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string SelectedTenantId { get; set; } = string.Empty;

    public Guid TenantId { get; set; }

    public string ActiveValue { get; set; } = "true";

    public bool Active { get; set; } = true;
}
