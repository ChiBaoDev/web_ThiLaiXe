namespace HeThongThiBangLai.Api.DTOs.Entitlements;

public class UpdateEntitlementPackageRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
