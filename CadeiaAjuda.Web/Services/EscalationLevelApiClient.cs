using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class EscalationLevelApiClient
{
    private readonly HttpClient _httpClient;

    public EscalationLevelApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<EscalationLevelViewModel>> GetAllAsync()
        => await _httpClient.GetFromJsonAsync<List<EscalationLevelViewModel>>("/api/escalation-levels") ?? [];

    public async Task<List<EscalationLevelViewModel>> GetBySectorIdAsync(Guid sectorId)
        => await _httpClient.GetFromJsonAsync<List<EscalationLevelViewModel>>($"/api/escalation-levels/by-sector/{sectorId}") ?? [];

    public async Task<EscalationLevelViewModel?> GetByIdAsync(Guid id)
        => await _httpClient.GetFromJsonAsync<EscalationLevelViewModel>($"/api/escalation-levels/{id}");

    public async Task<HttpResponseMessage> CreateAsync(EscalationLevelSaveModel model)
        => await _httpClient.PostAsJsonAsync("/api/escalation-levels", model);

    public async Task<HttpResponseMessage> UpdateAsync(Guid id, EscalationLevelSaveModel model)
        => await _httpClient.PutAsJsonAsync($"/api/escalation-levels/{id}", model);

    public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        => await _httpClient.DeleteAsync($"/api/escalation-levels/{id}");
}

public class EscalationLevelViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public int EscalationTimeMinutes { get; set; }
    public Guid SectorId { get; set; }
    public string SectorName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<EscalationLevelResponsibleViewModel> Responsibles { get; set; } = [];
}

public class EscalationLevelResponsibleViewModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class EscalationLevelSaveModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public int EscalationTimeMinutes { get; set; } = 30;
    public Guid SectorId { get; set; }
    public Guid TenantId { get; set; }
    public bool Active { get; set; } = true;
    public List<EscalationLevelResponsibleSaveModel> Responsibles { get; set; } = [];
}

public class EscalationLevelResponsibleSaveModel
{
    public Guid UserId { get; set; }
    public bool IsPrimary { get; set; }
}
