namespace HeThongThiBangLai.Api.DTOs.ExamSessions;

public class ExamSessionResultDto
{
    public long SessionId { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int UnansweredAnswers { get; set; }
    public decimal Score { get; set; }
    public string Result { get; set; } = string.Empty;
    public bool FailedByCriticalQuestion { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
