namespace CadeiaAjuda.ApiService.Application.DTOs;

public class DashboardDto
{
    // KPIs principais
    public int OpenCount { get; set; }
    public int ClosedTodayCount { get; set; }
    public int EscalatedCount { get; set; }
    public double AvgCloseTimeMinutes { get; set; }
    public int TotalCount { get; set; }
    public int ActiveUsersCount { get; set; }
    public int ActiveSectorsCount { get; set; }
    public double OldestOpenMinutes { get; set; }

    // Comparação com mês anterior
    public int CurrentMonthCount { get; set; }
    public int PreviousMonthCount { get; set; }
    public double MonthOverMonthChangePercent { get; set; }
    public int CurrentMonthClosedCount { get; set; }
    public int PreviousMonthClosedCount { get; set; }
    public double AvgCloseTimeCurrentMonth { get; set; }
    public double AvgCloseTimePreviousMonth { get; set; }

    // Chamados por período
    public List<DayCountDto> Last7Days { get; set; } = [];
    public List<DayCountDto> Last4Weeks { get; set; } = [];
    public List<DayCountDto> Last6Months { get; set; } = [];

    // Distribuição por status
    public List<StatusCountDto> StatusDistribution { get; set; } = [];

    // Ranking setores
    public List<SectorRankDto> SectorRanking { get; set; } = [];

    // Ranking tipos de chamado
    public List<TypeRankDto> TypeRanking { get; set; } = [];

    // Top solicitantes
    public List<UserRankDto> TopRequesters { get; set; } = [];

    // Chamados recentes
    public List<RecentRequestDto> RecentRequests { get; set; } = [];

    // Horários de pico
    public List<HourCountDto> PeakHours { get; set; } = [];
}

public class DayCountDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class StatusCountDto
{
    public int Status { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class SectorRankDto
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int OpenCount { get; set; }
    public double AvgCloseTimeMinutes { get; set; }
}

public class TypeRankDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class UserRankDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RecentRequestDto
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

public class HourCountDto
{
    public int Hour { get; set; }
    public int Count { get; set; }
}
