using CadeiaAjuda.ApiService.Domain.Entities;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetByCnpjAsync(string cnpj);
    Task<Tenant?> GetByIdentifierAsync(string identifier);
    Task<bool> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null);
    Task<bool> ExistsByIdentifierAsync(string identifier, Guid? excludeId = null);
}