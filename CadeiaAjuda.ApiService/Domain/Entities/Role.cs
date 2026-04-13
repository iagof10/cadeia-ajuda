namespace CadeiaAjuda.ApiService.Domain.Entities;

public class Role : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<User> Users { get; set; } = [];
}