namespace HeThongThiBangLai.Api.DTOs.Cms;

public class CreatePostRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Content { get; set; } = string.Empty;
    public string PostType { get; set; } = string.Empty;
    public long? ThumbnailFileId { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? CanonicalUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string Status { get; set; } = "draft";
    public List<long> CategoryIds { get; set; } = new();
}
