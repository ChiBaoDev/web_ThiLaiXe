namespace HeThongThiBangLai.Api.DTOs.Questions;

public class QuestionDto
{
    public long Id { get; set; }
    public long TopicId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public string? Level { get; set; }
    public bool IsCritical { get; set; }
    public string Status { get; set; } = string.Empty;
}
