namespace CadeiaAjuda.ApiService.Domain.Entities;

public class AndonUserSettings : EntityBase
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? AreaId { get; set; }
    public Area? Area { get; set; }

    public int WarningMinutes { get; set; } = 30;
    public int CriticalMinutes { get; set; } = 60;
    public int CarouselIntervalSeconds { get; set; } = 5;
    public bool ShowClock { get; set; } = true;
    public bool EnableSound { get; set; } = false;
}
