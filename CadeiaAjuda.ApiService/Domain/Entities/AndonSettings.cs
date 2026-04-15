namespace CadeiaAjuda.ApiService.Domain.Entities;

public class AndonSettings : TenantEntityBase
{
    public int WarningMinutes { get; set; } = 30;
    public int CriticalMinutes { get; set; } = 60;
}
