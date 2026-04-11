namespace CadeiaAjuda.ApiService.Domain.Entities;

public abstract class TenantEntityBase : EntityBase
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}