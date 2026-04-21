namespace HeThongThiBangLai.Api.DTOs.Dashboard;

public class DashboardOverviewDto
{
    public int TotalCandidates { get; set; }
    public int TotalSessions { get; set; }
    public decimal PassRate { get; set; }
    public decimal AverageScore { get; set; }
    public decimal CriticalFailRate { get; set; }
}
