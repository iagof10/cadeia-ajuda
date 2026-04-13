using CadeiaAjuda.ApiService.Domain.Entities;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public interface IUserSessionRepository
{
    Task<UserSession?> GetByTokenAsync(string sessionToken);
    Task<List<UserSession>> GetActiveByUserIdAsync(Guid userId);
    Task AddAsync(UserSession session);
    void Update(UserSession session);
    Task SaveChangesAsync();
}
