using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public class EscalationLevelRepository : Repository<EscalationLevel>, IEscalationLevelRepository
{
    public EscalationLevelRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<EscalationLevel>> GetAllWithIncludesAsync()
        => await _dbSet
            .Include(e => e.Tenant)
            .Include(e => e.Sector)
            .Include(e => e.Responsibles)
                .ThenInclude(r => r.User)
            .OrderBy(e => e.Sector.Name)
            .ThenBy(e => e.Order)
            .ToListAsync();

    public async Task<IEnumerable<EscalationLevel>> GetBySectorIdAsync(Guid sectorId)
        => await _dbSet
            .Include(e => e.Tenant)
            .Include(e => e.Sector)
            .Include(e => e.Responsibles)
                .ThenInclude(r => r.User)
            .Where(e => e.SectorId == sectorId)
            .OrderBy(e => e.Order)
            .ToListAsync();

    public async Task<EscalationLevel?> GetByIdWithIncludesAsync(Guid id)
        => await _dbSet
            .Include(e => e.Tenant)
            .Include(e => e.Sector)
            .Include(e => e.Responsibles)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<bool> ExistsByOrderAsync(Guid tenantId, Guid sectorId, int order, Guid? excludeId = null)
        => await _dbSet.AnyAsync(e =>
            e.TenantId == tenantId &&
            e.SectorId == sectorId &&
            e.Order == order &&
            (!excludeId.HasValue || e.Id != excludeId.Value));

    public void RemoveResponsibles(IEnumerable<EscalationLevelResponsible> responsibles)
        => _context.EscalationLevelResponsibles.RemoveRange(responsibles);
}
