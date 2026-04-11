namespace CadeiaAjuda.ApiService.Domain.Entities;

public class EscalationLevelResponsible : EntityBase
{
    public Guid EscalationLevelId { get; set; }
    public EscalationLevel EscalationLevel { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsPrimary { get; set; }
}
