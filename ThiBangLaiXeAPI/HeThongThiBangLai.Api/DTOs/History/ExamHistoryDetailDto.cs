namespace HeThongThiBangLai.Api.DTOs.History;

public class ExamHistoryDetailDto
{
    public long SessionId { get; set; }
    public long StudentId { get; set; }
    public long SampleExamId { get; set; }
    public string SampleExamName { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public decimal Score { get; set; }
    public string? Result { get; set; }
    public string Status { get; set; } = string.Empty;
}
