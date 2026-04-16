namespace CadeiaAjuda.ApiService.Domain.Entities;

public class HelpRequest : TenantEntityBase
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = null!;

    public Guid HelpRequestTypeId { get; set; }
    public HelpRequestType HelpRequestType { get; set; } = null!;

    public Guid AreaId { get; set; }
    public Area Area { get; set; } = null!;

    public Guid RequestedByUserId { get; set; }
    public User RequestedByUser { get; set; } = null!;

    public Guid? ClosedByUserId { get; set; }
    public User? ClosedByUser { get; set; }

    public HelpRequestStatus Status { get; set; } = HelpRequestStatus.Open;
    public DateTime? ClosedAt { get; set; }
}

public enum HelpRequestStatus
{
    Open = 0,
    Closed = 4
}
