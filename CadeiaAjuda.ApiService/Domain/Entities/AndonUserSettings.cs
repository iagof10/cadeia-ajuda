namespace CadeiaAjuda.ApiService.Domain.Entities;

public class AndonUserSettings : EntityBase
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int CarouselIntervalSeconds { get; set; } = 5;
    public bool ShowClock { get; set; } = true;
    public bool EnableSound { get; set; } = false;
}
