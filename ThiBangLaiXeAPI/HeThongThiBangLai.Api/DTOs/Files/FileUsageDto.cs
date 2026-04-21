namespace HeThongThiBangLai.Api.DTOs.Files;

public class FileUsageDto
{
    public long Id { get; set; }
    public long FileId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
