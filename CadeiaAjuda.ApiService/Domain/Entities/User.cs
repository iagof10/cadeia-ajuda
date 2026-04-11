namespace CadeiaAjuda.ApiService.Domain.Entities;

public class User : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}