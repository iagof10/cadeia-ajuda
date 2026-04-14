namespace CadeiaAjuda.ApiService.Application.DTOs;

public class HelpRequestTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid SectorId { get; set; }
    public string SectorName { get; set; } = string.Empty;
    public string SectorColor { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HelpRequestTypeCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid SectorId { get; set; }
    public Guid TenantId { get; set; }
}

public class HelpRequestTypeUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid SectorId { get; set; }
    public Guid TenantId { get; set; }
    public bool Active { get; set; }
}
