using CadeiaAjuda.ApiService.Domain.Entities;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public interface IHelpRequestRepository : IRepository<HelpRequest>
{
    Task<IEnumerable<HelpRequest>> GetAllWithIncludesAsync();
    Task<IEnumerable<HelpRequest>> GetByTenantIdAsync(Guid tenantId);
    Task<HelpRequest?> GetByIdWithIncludesAsync(Guid id);
    Task<int> CountByTenantAsync(Guid tenantId);
    void Remove(HelpRequest entity);
}
