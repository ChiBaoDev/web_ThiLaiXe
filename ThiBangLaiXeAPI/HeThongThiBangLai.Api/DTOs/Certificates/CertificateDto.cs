namespace HeThongThiBangLai.Api.DTOs.Certificates;

public class CertificateDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public long StudentId { get; set; }
    public long ExamResultId { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public long? CertificateFileId { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
