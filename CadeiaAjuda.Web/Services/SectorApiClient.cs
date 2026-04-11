using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class SectorApiClient
{
    private readonly HttpClient _httpClient;

    public SectorApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SectorViewModel>> GetAllAsync()
        => await _httpClient.GetFromJsonAsync<List<SectorViewModel>>("/api/sectors") ?? [];

    public async Task<SectorViewModel?> GetByIdAsync(Guid id)
        => await _httpClient.GetFromJsonAsync<SectorViewModel>($"/api/sectors/{id}");

    public async Task<HttpResponseMessage> CreateAsync(SectorFormModel model)
        => await _httpClient.PostAsJsonAsync("/api/sectors", model);

    public async Task<HttpResponseMessage> UpdateAsync(Guid id, SectorFormModel model)
        => await _httpClient.PutAsJsonAsync($"/api/sectors/{id}", model);

    public async Task<HttpResponseMessage> ToggleActiveAsync(Guid id)
        => await _httpClient.PatchAsync($"/api/sectors/{id}/toggle-active", null);
}

public class SectorViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#000000";
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SectorFormModel
{
    [Required(ErrorMessage = "Informe o nome do setor.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione uma cor.")]
    public string Color { get; set; } = "#1e9ff2";

    [Required(ErrorMessage = "Selecione a empresa.")]
    public string SelectedTenantId { get; set; } = string.Empty;

    public Guid TenantId { get; set; }

    public string ActiveValue { get; set; } = "true";

    public bool Active { get; set; } = true;
}
