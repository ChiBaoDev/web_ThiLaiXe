namespace HeThongThiBangLai.Api.DTOs.WrongQuestions;

public class WrongPracticeSessionDto
{
    public long SessionId { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime StartedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<long> QuestionIds { get; set; } = new();
}
