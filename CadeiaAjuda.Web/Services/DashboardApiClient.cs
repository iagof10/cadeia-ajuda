using System.Net.Http.Json;

namespace CadeiaAjuda.Web.Services;

public class DashboardApiClient
{
    private readonly HttpClient _http;

    public DashboardApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<DashboardViewModel?> GetDashboardAsync(Guid tenantId)
        => await _http.GetFromJsonAsync<DashboardViewModel>($"/api/dashboard/{tenantId}");
}

public class DashboardViewModel
{
    public int OpenCount { get; set; }
    public int ClosedTodayCount { get; set; }
    public int EscalatedCount { get; set; }
    public double AvgCloseTimeMinutes { get; set; }
    public int TotalCount { get; set; }
    public int ActiveUsersCount { get; set; }
    public int ActiveSectorsCount { get; set; }
    public double OldestOpenMinutes { get; set; }

    public int CurrentMonthCount { get; set; }
    public int PreviousMonthCount { get; set; }
    public double MonthOverMonthChangePercent { get; set; }
    public int CurrentMonthClosedCount { get; set; }
    public int PreviousMonthClosedCount { get; set; }
    public double AvgCloseTimeCurrentMonth { get; set; }
    public double AvgCloseTimePreviousMonth { get; set; }

    public List<DayCountItem> Last7Days { get; set; } = [];
    public List<DayCountItem> Last4Weeks { get; set; } = [];
    public List<DayCountItem> Last6Months { get; set; } = [];
    public List<StatusCountItem> StatusDistribution { get; set; } = [];
    public List<SectorRankItem> SectorRanking { get; set; } = [];
    public List<TypeRankItem> TypeRanking { get; set; } = [];
    public List<UserRankItem> TopRequesters { get; set; } = [];
    public List<RecentRequestItem> RecentRequests { get; set; } = [];
    public List<HourCountItem> PeakHours { get; set; } = [];
}

public class DayCountItem
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class StatusCountItem
{
    public int Status { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class SectorRankItem
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int OpenCount { get; set; }
    public double AvgCloseTimeMinutes { get; set; }
}

public class TypeRankItem
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class UserRankItem
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RecentRequestItem
{
    public string Code { get; set; } = string.Empty;
    public string SectorName { get; set; } = string.Empty;
    public string SectorColor { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}

public class HourCountItem
{
    public int Hour { get; set; }
    public int Count { get; set; }
}
