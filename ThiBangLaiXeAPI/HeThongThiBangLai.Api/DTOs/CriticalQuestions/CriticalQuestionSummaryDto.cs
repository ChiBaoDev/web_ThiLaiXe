namespace HeThongThiBangLai.Api.DTOs.CriticalQuestions;

public class CriticalQuestionSummaryDto
{
    public int TotalCriticalQuestions { get; set; }
    public int TotalPracticeSessions { get; set; }
    public DateTime? LatestPracticeAt { get; set; }
}
