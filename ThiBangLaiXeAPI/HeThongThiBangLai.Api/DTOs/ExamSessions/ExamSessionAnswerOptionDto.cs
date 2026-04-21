namespace HeThongThiBangLai.Api.DTOs.ExamSessions;

public class ExamSessionAnswerOptionDto
{
    public long AnswerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
}
