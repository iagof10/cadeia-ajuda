using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public class HelpRequestRepository : Repository<HelpRequest>, IHelpRequestRepository
{
    public HelpRequestRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<HelpRequest>> GetAllWithIncludesAsync()
        => await _dbSet
            .Include(h => h.Tenant)
            .Include(h => h.Sector)
            .Include(h => h.HelpRequestType)
            .Include(h => h.Area)
            .Include(h => h.RequestedByUser)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<HelpRequest>> GetByTenantIdAsync(Guid tenantId)
        => await _dbSet
            .Include(h => h.Sector)
            .Include(h => h.HelpRequestType)
            .Include(h => h.Area)
            .Include(h => h.RequestedByUser)
            .Where(h => h.TenantId == tenantId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();

    public async Task<HelpRequest?> GetByIdWithIncludesAsync(Guid id)
        => await _dbSet
            .Include(h => h.Tenant)
            .Include(h => h.Sector)
            .Include(h => h.HelpRequestType)
            .Include(h => h.Area)
            .Include(h => h.RequestedByUser)
            .FirstOrDefaultAsync(h => h.Id == id);

    public async Task<int> CountByTenantAsync(Guid tenantId)
        => await _dbSet.CountAsync(h => h.TenantId == tenantId);

    public void Remove(HelpRequest entity)
        => _dbSet.Remove(entity);
}
