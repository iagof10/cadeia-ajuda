namespace CadeiaAjuda.ApiService.Domain.Entities;

public class HelpRequestType : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DefaultPriorityLevel { get; set; }
    public int DefaultSlaMinutes { get; set; }
    public bool RequiresJustification { get; set; }

    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = null!;
}