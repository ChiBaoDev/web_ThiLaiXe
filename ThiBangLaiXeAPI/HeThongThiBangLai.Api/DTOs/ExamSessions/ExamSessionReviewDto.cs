namespace HeThongThiBangLai.Api.DTOs.ExamSessions;

public class ExamSessionReviewDto
{
    public long SessionId { get; set; }
    public List<ExamSessionReviewItemDto> Items { get; set; } = [];
}
