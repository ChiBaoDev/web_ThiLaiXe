namespace HeThongThiBangLai.Api.DTOs.Entitlements;

public class UserEntitlementDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long PackageId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
