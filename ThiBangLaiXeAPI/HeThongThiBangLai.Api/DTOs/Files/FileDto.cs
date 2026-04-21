namespace HeThongThiBangLai.Api.DTOs.Files;

public class FileDto
{
    public long Id { get; set; }
    public string StorageProvider { get; set; } = string.Empty;
    public string? BucketName { get; set; }
    public string ObjectKey { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string? ChecksumSha256 { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? DurationSeconds { get; set; }
    public string Status { get; set; } = string.Empty;
    public long? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
