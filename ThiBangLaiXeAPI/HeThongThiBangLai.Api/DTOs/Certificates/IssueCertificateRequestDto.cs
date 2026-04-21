namespace HeThongThiBangLai.Api.DTOs.Certificates;

public class IssueCertificateRequestDto
{
    public string Code { get; set; } = string.Empty;
    public long StudentId { get; set; }
    public long ExamResultId { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public long? CertificateFileId { get; set; }
}
