namespace HeThongThiBangLai.Api.DTOs.Exams;

public class UpdateSampleExamRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long ExamPeriodId { get; set; }
    public int TotalQuestions { get; set; }
    public int DurationMinutes { get; set; }
}
