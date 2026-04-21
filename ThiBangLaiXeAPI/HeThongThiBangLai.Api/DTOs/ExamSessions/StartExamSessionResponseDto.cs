namespace HeThongThiBangLai.Api.DTOs.ExamSessions;

public class StartExamSessionResponseDto
{
    public long SessionId { get; set; }
    public long SampleExamId { get; set; }
    public string SampleExamName { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime StartedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
