using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class TenantApiClient
{
    private readonly HttpClient _httpClient;

    public TenantApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TenantViewModel>> GetAllAsync()
        => await _httpClient.GetFromJsonAsync<List<TenantViewModel>>("/api/tenants") ?? [];

    public async Task<TenantViewModel?> GetByIdAsync(Guid id)
        => await _httpClient.GetFromJsonAsync<TenantViewModel>($"/api/tenants/{id}");

    public async Task<HttpResponseMessage> CreateAsync(TenantFormModel model)
        => await _httpClient.PostAsJsonAsync("/api/tenants", model);

    public async Task<HttpResponseMessage> UpdateAsync(Guid id, TenantFormModel model)
        => await _httpClient.PutAsJsonAsync($"/api/tenants/{id}", model);

    public async Task<HttpResponseMessage> ToggleActiveAsync(Guid id)
        => await _httpClient.PatchAsync($"/api/tenants/{id}/toggle-active", null);
}

public class TenantViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TenantFormModel
{
    [Required(ErrorMessage = "Informe o nome da empresa.")]
    public string Name { get; set; } = string.Empty;

    public string TradeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o CNPJ.")]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o identificador do tenant.")]
    public string Identifier { get; set; } = string.Empty;

    public string ActiveValue { get; set; } = "true";

    public bool Active { get; set; } = true;
}