using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public class HelpRequestTypeRepository : Repository<HelpRequestType>, IHelpRequestTypeRepository
{
    public HelpRequestTypeRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<HelpRequestType>> GetAllWithIncludesAsync()
        => await _dbSet
            .Include(h => h.Tenant)
            .Include(h => h.Sector)
            .OrderBy(h => h.Name)
            .ToListAsync();

    public async Task<HelpRequestType?> GetByIdWithIncludesAsync(Guid id)
        => await _dbSet
            .Include(h => h.Tenant)
            .Include(h => h.Sector)
            .FirstOrDefaultAsync(h => h.Id == id);

    public async Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null)
        => await _dbSet.AnyAsync(h => h.TenantId == tenantId && h.Name == name && (!excludeId.HasValue || h.Id != excludeId.Value));
}
