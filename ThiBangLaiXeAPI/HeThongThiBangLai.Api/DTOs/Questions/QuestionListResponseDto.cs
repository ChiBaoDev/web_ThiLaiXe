namespace HeThongThiBangLai.Api.DTOs.Questions;

public class QuestionListResponseDto
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public bool IsCritical { get; set; }
    public string Status { get; set; } = string.Empty;
}
