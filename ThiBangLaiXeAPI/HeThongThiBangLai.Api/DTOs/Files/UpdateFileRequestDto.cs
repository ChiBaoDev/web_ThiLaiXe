namespace HeThongThiBangLai.Api.DTOs.Files;

public class UpdateFileRequestDto
{
    public string? PublicUrl { get; set; }
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public long? SizeBytes { get; set; }
    public string? ChecksumSha256 { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Status { get; set; }
}
