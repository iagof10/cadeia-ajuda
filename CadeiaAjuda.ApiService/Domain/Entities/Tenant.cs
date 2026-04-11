namespace CadeiaAjuda.ApiService.Domain.Entities;

public class Tenant : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<User> Users { get; set; } = [];
    public ICollection<Role> Roles { get; set; } = [];
    public ICollection<Sector> Sectors { get; set; } = [];
    public ICollection<HelpRequestType> HelpRequestTypes { get; set; } = [];
    public ICollection<Priority> Priorities { get; set; } = [];
    public ICollection<EscalationLevel> EscalationLevels { get; set; } = [];
    public ICollection<Area> Areas { get; set; } = [];
    public ICollection<Reason> Reasons { get; set; } = [];
    public TenantSettings? Settings { get; set; }
}