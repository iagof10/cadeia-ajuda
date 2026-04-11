namespace CadeiaAjuda.ApiService.Domain.Entities;

public class Priority : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Color { get; set; } = "#000000";
    public int TargetMinutes { get; set; }
}