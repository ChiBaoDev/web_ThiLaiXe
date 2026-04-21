namespace HeThongThiBangLai.Api.DTOs;

public class khoa_hoc_dto
{
    public long id { get; set; }
    public string ma_khoa_hoc { get; set; }
    public string ten_khoa_hoc { get; set; }
    public string? mo_ta { get; set; }
    public decimal hoc_phi { get; set; }
    public int? thoi_luong { get; set; }
    public string trang_thai { get; set; }
}
