using CadeiaAjuda.ApiService.Domain.Entities;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public interface IAreaRepository : IRepository<Area>
{
    Task<IEnumerable<Area>> GetAllWithIncludesAsync();
    Task<Area?> GetByIdWithIncludesAsync(Guid id);
    Task<IEnumerable<Area>> GetByTenantIdAsync(Guid tenantId);
    Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? parentId, Guid? excludeId = null);
    Task<bool> HasChildrenAsync(Guid id);
    void Remove(Area entity);
}
