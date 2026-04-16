using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<User>> GetAllWithIncludesAsync()
        => await _dbSet
            .Include(u => u.Tenant)
            .Include(u => u.Role)
            .Include(u => u.UserSectors)
                .ThenInclude(us => us.Sector)
            .OrderBy(u => u.Name)
            .ToListAsync();

    public async Task<IEnumerable<User>> GetByTenantIdAsync(Guid tenantId)
        => await _dbSet
            .Include(u => u.Tenant)
            .Include(u => u.Role)
            .Include(u => u.UserSectors)
                .ThenInclude(us => us.Sector)
            .Where(u => u.TenantId == tenantId)
            .OrderBy(u => u.Name)
            .ToListAsync();

    public async Task<User?> GetByIdWithIncludesAsync(Guid id)
        => await _dbSet
            .Include(u => u.Tenant)
            .Include(u => u.Role)
                .ThenInclude(r => r!.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserSectors)
                .ThenInclude(us => us.Sector)
            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByLoginAsync(Guid tenantId, string login)
        => await _dbSet
            .Include(u => u.Tenant)
            .Include(u => u.Role)
                .ThenInclude(r => r!.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserSectors)
                .ThenInclude(us => us.Sector)
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Login == login && u.Active);

    public async Task<bool> ExistsByLoginAsync(Guid tenantId, string login, Guid? excludeId = null)
        => await _dbSet.AnyAsync(u => u.TenantId == tenantId && u.Login == login && (!excludeId.HasValue || u.Id != excludeId.Value));

    public async Task<bool> ExistsByEmailAsync(Guid tenantId, string email, Guid? excludeId = null)
        => await _dbSet.AnyAsync(u => u.TenantId == tenantId && u.Email == email && (!excludeId.HasValue || u.Id != excludeId.Value));

    public async Task<int> CountByTenantAndTypeAsync(Guid tenantId, UserType userType, Guid? excludeId = null)
        => await _dbSet.CountAsync(u => u.TenantId == tenantId && u.UserType == userType && (!excludeId.HasValue || u.Id != excludeId.Value));

    public async Task SetUserSectorsAsync(Guid userId, List<Guid> sectorIds)
    {
        var existing = await _context.UserSectors.Where(us => us.UserId == userId).ToListAsync();
        _context.UserSectors.RemoveRange(existing);

        if (sectorIds != null && sectorIds.Count > 0)
        {
            var newSectors = sectorIds.Select(sectorId => new UserSector
            {
                UserId = userId,
                SectorId = sectorId
            });
            await _context.UserSectors.AddRangeAsync(newSectors);
        }

        await _context.SaveChangesAsync();
    }
}
