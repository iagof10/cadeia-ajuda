using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public class SectorRepository : Repository<Sector>, ISectorRepository
{
    public SectorRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Sector>> GetAllWithIncludesAsync()
        => await _dbSet
            .Include(s => s.Tenant)
            .OrderBy(s => s.Name)
            .ToListAsync();

    public async Task<Sector?> GetByIdWithIncludesAsync(Guid id)
        => await _dbSet
            .Include(s => s.Tenant)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null)
        => await _dbSet.AnyAsync(s => s.TenantId == tenantId && s.Name == name && (!excludeId.HasValue || s.Id != excludeId.Value));
}
