using CadeiaAjuda.ApiService.Domain.Entities;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public interface ISectorRepository : IRepository<Sector>
{
    Task<IEnumerable<Sector>> GetAllWithIncludesAsync();
    Task<IEnumerable<Sector>> GetByTenantIdAsync(Guid tenantId);
    Task<Sector?> GetByIdWithIncludesAsync(Guid id);
    Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null);
}
