using CadeiaAjuda.ApiService.Application.DTOs;
using CadeiaAjuda.ApiService.Domain.Entities;
using CadeiaAjuda.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CadeiaAjuda.ApiService.Application.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(Guid tenantId);
}

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardDto> GetDashboardAsync(Guid tenantId)
    {
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var previousMonthStart = currentMonthStart.AddMonths(-1);
        var previousMonthEnd = currentMonthStart;

        // Load all requests for this tenant
        var requests = await _db.HelpRequests
            .Include(h => h.Sector)
            .Include(h => h.HelpRequestType)
            .Include(h => h.Area)
            .Include(h => h.RequestedByUser)
            .Include(h => h.ClosedByUser)
            .Where(h => h.TenantId == tenantId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();

        var open = requests.Where(r => r.Status <= HelpRequestStatus.Escalated).ToList();
        var escalated = requests.Where(r => r.Status == HelpRequestStatus.Escalated).ToList();
        var closedToday = requests.Where(r =>
            (r.Status == HelpRequestStatus.Closed || r.Status == HelpRequestStatus.Resolved)
            && r.ClosedAt.HasValue && r.ClosedAt.Value >= todayStart).ToList();

        // Avg close time (all closed requests that have both dates)
        var closedWithTimes = requests
            .Where(r => r.ClosedAt.HasValue && r.CreatedAt < r.ClosedAt.Value)
            .Select(r => (r.ClosedAt!.Value - r.CreatedAt).TotalMinutes)
            .ToList();
        var avgCloseTime = closedWithTimes.Count > 0 ? closedWithTimes.Average() : 0;

        // Oldest open
        var oldestOpen = open.OrderBy(r => r.CreatedAt).FirstOrDefault();
        var oldestMinutes = oldestOpen != null ? (now - oldestOpen.CreatedAt).TotalMinutes : 0;

        // Current month vs previous month
        var currentMonthRequests = requests.Where(r => r.CreatedAt >= currentMonthStart).ToList();
        var previousMonthRequests = requests.Where(r => r.CreatedAt >= previousMonthStart && r.CreatedAt < previousMonthEnd).ToList();
        var currentMonthClosed = requests.Where(r =>
            r.ClosedAt.HasValue && r.ClosedAt.Value >= currentMonthStart).ToList();
        var previousMonthClosed = requests.Where(r =>
            r.ClosedAt.HasValue && r.ClosedAt.Value >= previousMonthStart && r.ClosedAt.Value < previousMonthEnd).ToList();

        var avgCurrentMonth = currentMonthClosed
            .Where(r => r.ClosedAt.HasValue)
            .Select(r => (r.ClosedAt!.Value - r.CreatedAt).TotalMinutes)
            .DefaultIfEmpty(0).Average();
        var avgPreviousMonth = previousMonthClosed
            .Where(r => r.ClosedAt.HasValue)
            .Select(r => (r.ClosedAt!.Value - r.CreatedAt).TotalMinutes)
            .DefaultIfEmpty(0).Average();

        double momChange = 0;
        if (previousMonthRequests.Count > 0)
            momChange = Math.Round(((double)(currentMonthRequests.Count - previousMonthRequests.Count) / previousMonthRequests.Count) * 100, 1);

        // Active users and sectors
        var activeUsers = await _db.Users.CountAsync(u => u.TenantId == tenantId && u.Active);
        var activeSectors = await _db.Sectors.CountAsync(s => s.TenantId == tenantId && s.Active);

        // Last 7 days
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => todayStart.AddDays(-6 + i))
            .Select(d => new DayCountDto
            {
                Label = d.ToString("dd/MM"),
                Count = requests.Count(r => r.CreatedAt.Date == d.Date)
            }).ToList();

        // Last 4 weeks
        var last4Weeks = Enumerable.Range(0, 4)
            .Select(i =>
            {
                var weekStart = todayStart.AddDays(-((int)todayStart.DayOfWeek) - (3 - i) * 7);
                var weekEnd = weekStart.AddDays(7);
                return new DayCountDto
                {
                    Label = $"{weekStart:dd/MM} - {weekEnd.AddDays(-1):dd/MM}",
                    Count = requests.Count(r => r.CreatedAt >= weekStart && r.CreatedAt < weekEnd)
                };
            }).ToList();

        // Last 6 months
        var last6Months = Enumerable.Range(0, 6)
            .Select(i =>
            {
                var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-5 + i);
                var monthEnd = monthStart.AddMonths(1);
                return new DayCountDto
                {
                    Label = monthStart.ToString("MMM/yy"),
                    Count = requests.Count(r => r.CreatedAt >= monthStart && r.CreatedAt < monthEnd)
                };
            }).ToList();

        // Status distribution
        var statusDistribution = Enum.GetValues<HelpRequestStatus>()
            .Select(s => new StatusCountDto
            {
                Status = (int)s,
                Name = StatusName(s),
                Count = requests.Count(r => r.Status == s)
            }).ToList();

        // Sector ranking
        var sectorRanking = requests
            .GroupBy(r => new { r.SectorId, r.Sector?.Name, r.Sector?.Color })
            .Select(g =>
            {
                var closed = g.Where(r => r.ClosedAt.HasValue && r.CreatedAt < r.ClosedAt.Value).ToList();
                return new SectorRankDto
                {
                    Name = g.Key.Name ?? "",
                    Color = g.Key.Color ?? "#999",
                    TotalCount = g.Count(),
                    OpenCount = g.Count(r => r.Status <= HelpRequestStatus.Escalated),
                    AvgCloseTimeMinutes = closed.Count > 0
                        ? Math.Round(closed.Average(r => (r.ClosedAt!.Value - r.CreatedAt).TotalMinutes), 1)
                        : 0
                };
            })
            .OrderByDescending(s => s.TotalCount)
            .Take(10)
            .ToList();

        // Type ranking
        var typeRanking = requests
            .GroupBy(r => r.HelpRequestType?.Name ?? "")
            .Select(g => new TypeRankDto { Name = g.Key, Count = g.Count() })
            .OrderByDescending(t => t.Count)
            .Take(10)
            .ToList();

        // Top requesters
        var topRequesters = requests
            .GroupBy(r => r.RequestedByUser?.Name ?? "")
            .Select(g => new UserRankDto { Name = g.Key, Count = g.Count() })
            .OrderByDescending(u => u.Count)
            .Take(10)
            .ToList();

        // Recent requests
        var recentRequests = requests.Take(10).Select(r => new RecentRequestDto
        {
            Code = r.Code,
            SectorName = r.Sector?.Name ?? "",
            SectorColor = r.Sector?.Color ?? "#999",
            TypeName = r.HelpRequestType?.Name ?? "",
            AreaName = r.Area?.Name ?? "",
            RequestedBy = r.RequestedByUser?.Name ?? "",
            Status = (int)r.Status,
            StatusName = StatusName(r.Status),
            CreatedAt = r.CreatedAt,
            ClosedAt = r.ClosedAt
        }).ToList();

        // Peak hours
        var peakHours = Enumerable.Range(0, 24)
            .Select(h => new HourCountDto
            {
                Hour = h,
                Count = requests.Count(r => r.CreatedAt.Hour == h)
            }).ToList();

        return new DashboardDto
        {
            OpenCount = open.Count,
            ClosedTodayCount = closedToday.Count,
            EscalatedCount = escalated.Count,
            AvgCloseTimeMinutes = Math.Round(avgCloseTime, 1),
            TotalCount = requests.Count,
            ActiveUsersCount = activeUsers,
            ActiveSectorsCount = activeSectors,
            OldestOpenMinutes = Math.Round(oldestMinutes, 1),
            CurrentMonthCount = currentMonthRequests.Count,
            PreviousMonthCount = previousMonthRequests.Count,
            MonthOverMonthChangePercent = momChange,
            CurrentMonthClosedCount = currentMonthClosed.Count,
            PreviousMonthClosedCount = previousMonthClosed.Count,
            AvgCloseTimeCurrentMonth = Math.Round(avgCurrentMonth, 1),
            AvgCloseTimePreviousMonth = Math.Round(avgPreviousMonth, 1),
            Last7Days = last7Days,
            Last4Weeks = last4Weeks,
            Last6Months = last6Months,
            StatusDistribution = statusDistribution,
            SectorRanking = sectorRanking,
            TypeRanking = typeRanking,
            TopRequesters = topRequesters,
            RecentRequests = recentRequests,
            PeakHours = peakHours
        };
    }

    private static string StatusName(HelpRequestStatus status) => status switch
    {
        HelpRequestStatus.Open => "Aberto",
        HelpRequestStatus.InProgress => "Em Andamento",
        HelpRequestStatus.Escalated => "Escalado",
        HelpRequestStatus.Resolved => "Resolvido",
        HelpRequestStatus.Closed => "Encerrado",
        _ => "Desconhecido"
    };
}
