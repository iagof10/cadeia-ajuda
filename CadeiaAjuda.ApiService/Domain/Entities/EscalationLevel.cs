namespace CadeiaAjuda.ApiService.Domain.Entities;

public class EscalationLevel : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public int EscalationTimeMinutes { get; set; }

    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = null!;

    public ICollection<EscalationLevelResponsible> Responsibles { get; set; } = [];
}