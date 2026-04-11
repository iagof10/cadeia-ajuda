namespace CadeiaAjuda.ApiService.Domain.Entities;

public class Area : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Area? Parent { get; set; }
    public ICollection<Area> Children { get; set; } = [];
}
