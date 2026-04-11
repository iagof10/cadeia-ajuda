namespace CadeiaAjuda.ApiService.Domain.Entities;

public class TenantSettings : TenantEntityBase
{
    public string DisplayName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#666ee8";
    public int DefaultSlaMinutes { get; set; } = 30;
    public bool EnableNotifications { get; set; } = true;
    public bool EnableAutoEscalation { get; set; } = true;
    public bool EnableAndonPanel { get; set; } = true;
    public string TimeZone { get; set; } = "America/Sao_Paulo";
    public string Language { get; set; } = "pt-BR";
}