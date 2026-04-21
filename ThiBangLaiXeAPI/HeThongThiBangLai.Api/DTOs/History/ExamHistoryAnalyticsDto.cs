namespace HeThongThiBangLai.Api.DTOs.History;

public class ExamHistoryAnalyticsDto
{
    public int TotalSessions { get; set; }
    public int PassedSessions { get; set; }
    public int FailedSessions { get; set; }
    public decimal AverageScore { get; set; }
    public decimal PassRate { get; set; }
}
