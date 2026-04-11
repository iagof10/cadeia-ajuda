using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public class ReasonRepository : Repository<Reason>, IReasonRepository
{
    public ReasonRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Reason>> GetAllWithIncludesAsync()
        => await _dbSet
            .Include(r => r.Tenant)
            .OrderBy(r => r.Name)
            .ToListAsync();

    public async Task<Reason?> GetByIdWithIncludesAsync(Guid id)
        => await _dbSet
            .Include(r => r.Tenant)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null)
        => await _dbSet.AnyAsync(r => r.TenantId == tenantId && r.Name == name && (!excludeId.HasValue || r.Id != excludeId.Value));
}
