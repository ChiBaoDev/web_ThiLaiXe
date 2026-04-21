namespace HeThongThiBangLai.Api.DTOs.WrongQuestions;

public class WrongQuestionDto
{
    public long QuestionId { get; set; }
    public long TopicId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Level { get; set; }
    public int WrongCount { get; set; }
}
