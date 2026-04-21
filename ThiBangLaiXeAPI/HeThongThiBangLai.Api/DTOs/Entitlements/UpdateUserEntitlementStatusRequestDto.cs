namespace HeThongThiBangLai.Api.DTOs.Entitlements;

public class UpdateUserEntitlementStatusRequestDto
{
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
}
