namespace HeThongThiBangLai.Api.DTOs.Topics;

public class CreateTopicRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}