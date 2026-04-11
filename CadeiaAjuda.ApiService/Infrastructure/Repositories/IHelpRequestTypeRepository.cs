using CadeiaAjuda.ApiService.Domain.Entities;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public interface IHelpRequestTypeRepository : IRepository<HelpRequestType>
{
    Task<IEnumerable<HelpRequestType>> GetAllWithIncludesAsync();
    Task<IEnumerable<HelpRequestType>> GetByTenantIdAsync(Guid tenantId);
    Task<HelpRequestType?> GetByIdWithIncludesAsync(Guid id);
    Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null);
}
