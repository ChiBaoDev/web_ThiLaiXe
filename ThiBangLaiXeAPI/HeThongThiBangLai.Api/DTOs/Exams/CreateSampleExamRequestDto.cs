namespace HeThongThiBangLai.Api.DTOs.Exams;

public class CreateSampleExamRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long ExamPeriodId { get; set; }
    public int TotalQuestions { get; set; }
    public int DurationMinutes { get; set; }
}
