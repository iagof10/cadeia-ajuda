using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(AppDbContext context) : base(context) { }

    public async Task<Tenant?> GetByCnpjAsync(string cnpj)
        => await _dbSet.FirstOrDefaultAsync(t => t.Cnpj == cnpj);

    public async Task<Tenant?> GetByIdentifierAsync(string identifier)
        => await _dbSet.FirstOrDefaultAsync(t => t.Identifier == identifier);

    public async Task<bool> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null)
        => await _dbSet.AnyAsync(t => t.Cnpj == cnpj && (!excludeId.HasValue || t.Id != excludeId.Value));

    public async Task<bool> ExistsByIdentifierAsync(string identifier, Guid? excludeId = null)
        => await _dbSet.AnyAsync(t => t.Identifier == identifier && (!excludeId.HasValue || t.Id != excludeId.Value));
}