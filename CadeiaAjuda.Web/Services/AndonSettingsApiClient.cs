using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class AndonSettingsApiClient
{
    private readonly HttpClient _httpClient;

    public AndonSettingsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AndonSettingsViewModel?> GetByTenantIdAsync(Guid tenantId)
        => await _httpClient.GetFromJsonAsync<AndonSettingsViewModel>($"/api/andon-settings/{tenantId}");

    public async Task<HttpResponseMessage> UpdateAsync(Guid tenantId, AndonSettingsFormModel model)
        => await _httpClient.PutAsJsonAsync($"/api/andon-settings/{tenantId}", model);
}

public class AndonSettingsViewModel
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public int WarningMinutes { get; set; }
    public int CriticalMinutes { get; set; }
}

public class AndonSettingsFormModel
{
    public int WarningMinutes { get; set; } = 30;
    public int CriticalMinutes { get; set; } = 60;
}
