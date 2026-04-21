namespace HeThongThiBangLai.Api.DTOs.Files;

public class FileListResponseDto
{
    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StorageProvider { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
