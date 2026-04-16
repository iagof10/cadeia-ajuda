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
            .Include(h => h.ClosedByUser)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<HelpRequest>> GetByTenantIdAsync(Guid tenantId)
        => await _dbSet
            .Include(h => h.Sector)
            .Include(h => h.HelpRequestType)
            .Include(h => h.Area)
            .Include(h => h.RequestedByUser)
            .Include(h => h.ClosedByUser)
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
            .Include(h => h.ClosedByUser)
            .FirstOrDefaultAsync(h => h.Id == id);

    public async Task<int> CountByTenantAsync(Guid tenantId)
        => await _dbSet.CountAsync(h => h.TenantId == tenantId);

    public async Task<bool> HasOpenBySectorAndAreaAsync(Guid tenantId, Guid sectorId, Guid areaId)
        => await _dbSet.AnyAsync(h =>
            h.TenantId == tenantId &&
            h.SectorId == sectorId &&
            h.AreaId == areaId &&
            h.Status == HelpRequestStatus.Open);

    public void Remove(HelpRequest entity)
        => _dbSet.Remove(entity);
}
