namespace HeThongThiBangLai.Api.DTOs.CriticalQuestions;

public class CriticalPracticeSessionDto
{
    public long SessionId { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime StartedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<long> QuestionIds { get; set; } = new();
}
