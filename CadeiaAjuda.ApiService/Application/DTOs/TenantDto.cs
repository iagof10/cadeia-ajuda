namespace CadeiaAjuda.ApiService.Application.DTOs;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? StandardUserLimit { get; set; }
    public int? AndonUserLimit { get; set; }
    public int? ManagerUserLimit { get; set; }
    public int? AdministratorUserLimit { get; set; }
}

public class TenantCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public int? StandardUserLimit { get; set; }
    public int? AndonUserLimit { get; set; }
    public int? ManagerUserLimit { get; set; }
    public int? AdministratorUserLimit { get; set; }
}

public class TenantUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public bool Active { get; set; }
    public int? StandardUserLimit { get; set; }
    public int? AndonUserLimit { get; set; }
    public int? ManagerUserLimit { get; set; }
    public int? AdministratorUserLimit { get; set; }
}