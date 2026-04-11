using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public class AreaRepository : Repository<Area>, IAreaRepository
{
    public AreaRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Area>> GetAllWithIncludesAsync()
        => await _dbSet
            .Include(a => a.Tenant)
            .OrderBy(a => a.Name)
            .ToListAsync();

    public async Task<Area?> GetByIdWithIncludesAsync(Guid id)
        => await _dbSet
            .Include(a => a.Tenant)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Area>> GetByTenantIdAsync(Guid tenantId)
        => await _dbSet
            .Include(a => a.Tenant)
            .Where(a => a.TenantId == tenantId)
            .OrderBy(a => a.Name)
            .ToListAsync();

    public async Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? parentId, Guid? excludeId = null)
        => await _dbSet.AnyAsync(a =>
            a.TenantId == tenantId &&
            a.Name == name &&
            a.ParentId == parentId &&
            (!excludeId.HasValue || a.Id != excludeId.Value));

    public async Task<bool> HasChildrenAsync(Guid id)
        => await _dbSet.AnyAsync(a => a.ParentId == id);

    public void Remove(Area entity)
        => _dbSet.Remove(entity);
}
