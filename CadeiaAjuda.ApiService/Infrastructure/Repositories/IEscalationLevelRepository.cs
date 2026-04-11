using CadeiaAjuda.ApiService.Domain.Entities;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public interface IEscalationLevelRepository : IRepository<EscalationLevel>
{
    Task<IEnumerable<EscalationLevel>> GetAllWithIncludesAsync();
    Task<IEnumerable<EscalationLevel>> GetBySectorIdAsync(Guid sectorId);
    Task<EscalationLevel?> GetByIdWithIncludesAsync(Guid id);
    Task<bool> ExistsByOrderAsync(Guid tenantId, Guid sectorId, int order, Guid? excludeId = null);
    void RemoveResponsibles(IEnumerable<EscalationLevelResponsible> responsibles);
}
