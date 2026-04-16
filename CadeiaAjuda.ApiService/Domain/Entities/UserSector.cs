namespace CadeiaAjuda.ApiService.Domain.Entities;

public class UserSector
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = null!;
}
