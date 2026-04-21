namespace HeThongThiBangLai.Api.DTOs.Topics;

public class TopicDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int QuestionCount { get; set; }
}
