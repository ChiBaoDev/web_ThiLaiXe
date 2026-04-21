namespace HeThongThiBangLai.Api.DTOs.Dashboard;

public class DashboardExamStatsDto
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int TotalSessions { get; set; }
    public int PassedSessions { get; set; }
    public int FailedSessions { get; set; }
    public decimal PassRate { get; set; }
    public decimal AverageScore { get; set; }
    public List<DashboardTrendPointDto> DailyTrend { get; set; } = new();
}

public class DashboardTrendPointDto
{
    public DateTime Date { get; set; }
    public int SessionCount { get; set; }
}
