namespace HeThongThiBangLai.Api.DTOs.Cms;

public class UpdateCategoryRequestDto
{
    public long? ParentId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
