namespace HeThongThiBangLai.Api.DTOs.Questions;

public class UpdateQuestionRequestDto
{
    public long TopicId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public string? Level { get; set; }
    public bool IsCritical { get; set; }
}
