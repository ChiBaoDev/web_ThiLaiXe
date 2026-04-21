namespace HeThongThiBangLai.Api.DTOs.Dashboard;

public class DashboardQuestionStatsDto
{
    public int TotalAnsweredQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public decimal AccuracyRate { get; set; }
    public List<DashboardQuestionErrorDto> MostWrongQuestions { get; set; } = new();
}

public class DashboardQuestionErrorDto
{
    public long QuestionId { get; set; }
    public string QuestionContent { get; set; } = string.Empty;
    public int WrongCount { get; set; }
}
