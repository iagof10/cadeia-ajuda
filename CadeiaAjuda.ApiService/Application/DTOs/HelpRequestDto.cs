namespace CadeiaAjuda.ApiService.Application.DTOs;

public class HelpRequestDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid SectorId { get; set; }
    public string SectorName { get; set; } = string.Empty;
    public string SectorColor { get; set; } = string.Empty;
    public Guid HelpRequestTypeId { get; set; }
    public string HelpRequestTypeName { get; set; } = string.Empty;
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public Guid RequestedByUserId { get; set; }
    public string RequestedByUserName { get; set; } = string.Empty;
    public Guid? ClosedByUserId { get; set; }
    public string ClosedByUserName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public bool Active { get; set; }
}

public class HelpRequestCreateDto
{
    public string Description { get; set; } = string.Empty;
    public Guid SectorId { get; set; }
    public Guid HelpRequestTypeId { get; set; }
    public Guid AreaId { get; set; }
    public Guid RequestedByUserId { get; set; }
    public Guid TenantId { get; set; }
}

public class HelpRequestCloseDto
{
    public Guid ClosedByUserId { get; set; }
}
