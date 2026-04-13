using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Infrastructure.Repositories;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly AppDbContext _context;

    public UserSessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserSession?> GetByTokenAsync(string sessionToken)
        => await _context.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);

    public async Task<List<UserSession>> GetActiveByUserIdAsync(Guid userId)
        => await _context.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync();

    public async Task AddAsync(UserSession session)
        => await _context.UserSessions.AddAsync(session);

    public void Update(UserSession session)
        => _context.UserSessions.Update(session);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
