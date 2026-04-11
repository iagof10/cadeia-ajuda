namespace CadeiaAjuda.ApiService.Domain.Entities;

public class Sector : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = "#000000";
}