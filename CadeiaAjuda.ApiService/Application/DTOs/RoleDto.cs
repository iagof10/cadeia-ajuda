namespace CadeiaAjuda.ApiService.Application.DTOs;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Permissions { get; set; } = [];
}

public class RoleCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public List<string> Permissions { get; set; } = [];
}

public class RoleUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public bool Active { get; set; }
    public List<string> Permissions { get; set; } = [];
}
