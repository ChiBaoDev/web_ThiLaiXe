namespace HeThongThiBangLai.Api.DTOs;

public class diem_danh_dto
{
    public long id { get; set; }
    public long buoi_hoc_id { get; set; }
    public long hoc_vien_id { get; set; }
    public string trang_thai { get; set; }
    public string? ghi_chu { get; set; }
    public long? giao_vien_id { get; set; }
    public DateTime thoi_gian_diem_danh { get; set; }
}
