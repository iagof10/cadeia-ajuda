namespace CadeiaAjuda.ApiService.Domain.Entities;

public class Reason : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
