namespace HeThongThiBangLai.Api.DTOs.Exams;

public class SampleExamDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long ExamPeriodId { get; set; }
    public int TotalQuestions { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int LinkedQuestionCount { get; set; }
    public List<long> QuestionIds { get; set; } = new();
}
