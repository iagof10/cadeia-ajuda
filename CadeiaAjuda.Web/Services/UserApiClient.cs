using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class UserApiClient
{
    private readonly HttpClient _httpClient;

    public UserApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserViewModel>> GetAllAsync()
        => await _httpClient.GetFromJsonAsync<List<UserViewModel>>("/api/users") ?? [];

    public async Task<List<UserViewModel>> GetByTenantIdAsync(Guid tenantId)
        => await _httpClient.GetFromJsonAsync<List<UserViewModel>>($"/api/users/by-tenant/{tenantId}") ?? [];

    public async Task<UserViewModel?> GetByIdAsync(Guid id)
        => await _httpClient.GetFromJsonAsync<UserViewModel>($"/api/users/{id}");

    public async Task<HttpResponseMessage> CreateAsync(UserFormModel model)
        => await _httpClient.PostAsJsonAsync("/api/users", model);

    public async Task<HttpResponseMessage> UpdateAsync(Guid id, UserFormModel model)
        => await _httpClient.PutAsJsonAsync($"/api/users/{id}", model);

    public async Task<HttpResponseMessage> ToggleActiveAsync(Guid id)
        => await _httpClient.PatchAsync($"/api/users/{id}/toggle-active", null);
}

public class UserViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public string UserTypeName { get; set; } = string.Empty;
}

public class UserFormModel
{
    [Required(ErrorMessage = "Informe o nome do usuário.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o login.")]
    public string Login { get; set; } = string.Empty;

    public string? Password { get; set; }

    public string SelectedTenantId { get; set; } = string.Empty;

    public Guid TenantId { get; set; }

    public string ActiveValue { get; set; } = "true";

    public bool Active { get; set; } = true;

    public string? SelectedRoleId { get; set; }
    public Guid? RoleId { get; set; }
    public UserType UserType { get; set; } = UserType.Standard;
}
