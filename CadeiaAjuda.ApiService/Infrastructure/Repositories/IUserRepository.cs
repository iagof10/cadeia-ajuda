using CadeiaAjuda.ApiService.Domain.Entities;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<IEnumerable<User>> GetAllWithIncludesAsync();
    Task<IEnumerable<User>> GetByTenantIdAsync(Guid tenantId);
    Task<User?> GetByIdWithIncludesAsync(Guid id);
    Task<User?> GetByLoginAsync(Guid tenantId, string login);
    Task<bool> ExistsByLoginAsync(Guid tenantId, string login, Guid? excludeId = null);
    Task<bool> ExistsByEmailAsync(Guid tenantId, string email, Guid? excludeId = null);
    Task<int> CountByTenantAndTypeAsync(Guid tenantId, UserType userType, Guid? excludeId = null);
    Task SetUserSectorsAsync(Guid userId, List<Guid> sectorIds);
}
