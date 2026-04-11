using CadeiaAjuda.ApiService.Domain.Entities;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public interface IReasonRepository : IRepository<Reason>
{
    Task<IEnumerable<Reason>> GetAllWithIncludesAsync();
    Task<Reason?> GetByIdWithIncludesAsync(Guid id);
    Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null);
}
