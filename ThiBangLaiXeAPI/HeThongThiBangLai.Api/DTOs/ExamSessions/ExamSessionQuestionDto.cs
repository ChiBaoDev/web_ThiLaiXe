namespace HeThongThiBangLai.Api.DTOs.ExamSessions;

public class ExamSessionQuestionDto
{
    public int Number { get; set; }
    public long QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public long TopicId { get; set; }
    public bool IsCritical { get; set; }
    public long? SelectedAnswerId { get; set; }
    public List<ExamSessionAnswerOptionDto> Answers { get; set; } = [];
}
