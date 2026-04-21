namespace HeThongThiBangLai.Api.DTOs.Dashboard;

public class DashboardCriticalQuestionStatsDto
{
    public int TotalCriticalAttempts { get; set; }
    public int WrongCriticalAttempts { get; set; }
    public decimal CriticalErrorRate { get; set; }
    public List<DashboardQuestionErrorDto> TopCriticalWrongQuestions { get; set; } = new();
}
