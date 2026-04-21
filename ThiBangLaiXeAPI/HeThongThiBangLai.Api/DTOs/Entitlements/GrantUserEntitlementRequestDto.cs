namespace HeThongThiBangLai.Api.DTOs.Entitlements;

public class GrantUserEntitlementRequestDto
{
    public long UserId { get; set; }
    public long PackageId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? Note { get; set; }
}
