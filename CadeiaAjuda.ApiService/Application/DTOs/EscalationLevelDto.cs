namespace CadeiaAjuda.ApiService.Application.DTOs;

public class EscalationLevelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public int EscalationTimeMinutes { get; set; }
    public Guid SectorId { get; set; }
    public string SectorName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<EscalationLevelResponsibleDto> Responsibles { get; set; } = [];
}

public class EscalationLevelResponsibleDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class EscalationLevelCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public int EscalationTimeMinutes { get; set; }
    public Guid SectorId { get; set; }
    public Guid TenantId { get; set; }
    public List<EscalationLevelResponsibleCreateDto> Responsibles { get; set; } = [];
}

public class EscalationLevelUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public int EscalationTimeMinutes { get; set; }
    public Guid SectorId { get; set; }
    public Guid TenantId { get; set; }
    public bool Active { get; set; }
    public List<EscalationLevelResponsibleCreateDto> Responsibles { get; set; } = [];
}

public class EscalationLevelResponsibleCreateDto
{
    public Guid UserId { get; set; }
    public bool IsPrimary { get; set; }
}
