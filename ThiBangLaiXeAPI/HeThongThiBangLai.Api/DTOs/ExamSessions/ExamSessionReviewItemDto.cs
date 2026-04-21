namespace HeThongThiBangLai.Api.DTOs.ExamSessions;

public class ExamSessionReviewItemDto
{
    public int Number { get; set; }
    public long QuestionId { get; set; }
    public string QuestionContent { get; set; } = string.Empty;
    public bool IsCritical { get; set; }
    public long? SelectedAnswerId { get; set; }
    public long? CorrectAnswerId { get; set; }
    public bool? IsCorrect { get; set; }
}
