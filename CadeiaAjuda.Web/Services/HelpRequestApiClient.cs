using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class HelpRequestApiClient
{
    private readonly HttpClient _http;

    public HelpRequestApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<HelpRequestViewModel>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<HelpRequestViewModel>>("/api/help-requests") ?? [];

    public async Task<List<HelpRequestViewModel>> GetByTenantIdAsync(Guid tenantId)
        => await _http.GetFromJsonAsync<List<HelpRequestViewModel>>($"/api/help-requests/by-tenant/{tenantId}") ?? [];

    public async Task<HelpRequestViewModel?> GetByIdAsync(Guid id)
        => await _http.GetFromJsonAsync<HelpRequestViewModel>($"/api/help-requests/{id}");

    public async Task<HttpResponseMessage> CreateAsync(HelpRequestCreateModel model)
        => await _http.PostAsJsonAsync("/api/help-requests", model);
}

public class HelpRequestViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid SectorId { get; set; }
    public string SectorName { get; set; } = string.Empty;
    public string SectorColor { get; set; } = string.Empty;
    public Guid HelpRequestTypeId { get; set; }
    public string HelpRequestTypeName { get; set; } = string.Empty;
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public Guid RequestedByUserId { get; set; }
    public string RequestedByUserName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public bool Active { get; set; }
}

public class HelpRequestCreateModel
{
    public string Description { get; set; } = string.Empty;
    public Guid SectorId { get; set; }
    public Guid HelpRequestTypeId { get; set; }
    public Guid AreaId { get; set; }
    public Guid RequestedByUserId { get; set; }
    public Guid TenantId { get; set; }
}
