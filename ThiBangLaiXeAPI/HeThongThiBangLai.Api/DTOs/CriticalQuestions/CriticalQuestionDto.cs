namespace HeThongThiBangLai.Api.DTOs.CriticalQuestions;

public class CriticalQuestionDto
{
    public long Id { get; set; }
    public long TopicId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public string? Level { get; set; }
}
