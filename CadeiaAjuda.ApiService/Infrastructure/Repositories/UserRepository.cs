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
            .OrderBy(u => u.Name)
            .ToListAsync();

    public async Task<IEnumerable<User>> GetByTenantIdAsync(Guid tenantId)
        => await _dbSet
            .Include(u => u.Tenant)
            .Where(u => u.TenantId == tenantId)
            .OrderBy(u => u.Name)
            .ToListAsync();

    public async Task<User?> GetByIdWithIncludesAsync(Guid id)
        => await _dbSet
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByLoginAsync(Guid tenantId, string login)
        => await _dbSet
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Login == login && u.Active);

    public async Task<bool> ExistsByLoginAsync(Guid tenantId, string login, Guid? excludeId = null)
        => await _dbSet.AnyAsync(u => u.TenantId == tenantId && u.Login == login && (!excludeId.HasValue || u.Id != excludeId.Value));

    public async Task<bool> ExistsByEmailAsync(Guid tenantId, string email, Guid? excludeId = null)
        => await _dbSet.AnyAsync(u => u.TenantId == tenantId && u.Email == email && (!excludeId.HasValue || u.Id != excludeId.Value));
}
