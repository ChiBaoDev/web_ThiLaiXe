namespace HeThongThiBangLai.Api.DTOs.WrongQuestions;

public class WrongQuestionSummaryDto
{
    public int TotalWrongQuestions { get; set; }
    public int UnresolvedQuestions { get; set; }
    public int ResolvedQuestions { get; set; }
    public int TotalPracticeSessions { get; set; }
    public DateTime? LatestPracticeAt { get; set; }
}
