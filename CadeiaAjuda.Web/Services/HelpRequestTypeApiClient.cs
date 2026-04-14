using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class HelpRequestTypeApiClient
{
    private readonly HttpClient _httpClient;

    public HelpRequestTypeApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<HelpRequestTypeViewModel>> GetAllAsync()
        => await _httpClient.GetFromJsonAsync<List<HelpRequestTypeViewModel>>("/api/help-request-types") ?? [];

    public async Task<List<HelpRequestTypeViewModel>> GetByTenantIdAsync(Guid tenantId)
        => await _httpClient.GetFromJsonAsync<List<HelpRequestTypeViewModel>>($"/api/help-request-types/by-tenant/{tenantId}") ?? [];

    public async Task<HelpRequestTypeViewModel?> GetByIdAsync(Guid id)
        => await _httpClient.GetFromJsonAsync<HelpRequestTypeViewModel>($"/api/help-request-types/{id}");

    public async Task<HttpResponseMessage> CreateAsync(HelpRequestTypeFormModel model)
        => await _httpClient.PostAsJsonAsync("/api/help-request-types", model);

    public async Task<HttpResponseMessage> UpdateAsync(Guid id, HelpRequestTypeFormModel model)
        => await _httpClient.PutAsJsonAsync($"/api/help-request-types/{id}", model);

    public async Task<HttpResponseMessage> ToggleActiveAsync(Guid id)
        => await _httpClient.PatchAsync($"/api/help-request-types/{id}/toggle-active", null);
}

public class HelpRequestTypeViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid SectorId { get; set; }
    public string SectorName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HelpRequestTypeFormModel
{
    [Required(ErrorMessage = "Informe o nome do tipo de solicitação.")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o setor.")]
    public string SelectedSectorId { get; set; } = string.Empty;

    public Guid SectorId { get; set; }

    public string SelectedTenantId { get; set; } = string.Empty;

    public Guid TenantId { get; set; }

    public string ActiveValue { get; set; } = "true";

    public bool Active { get; set; } = true;
}
