namespace HeThongThiBangLai.Api.DTOs.Cms;

public class PostListResponseDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string PostType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
