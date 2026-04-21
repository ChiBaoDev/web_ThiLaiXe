namespace HeThongThiBangLai.Api.DTOs;

public class ky_thi_dto
{
    public long id { get; set; }
    public string ma_ky_thi { get; set; }
    public string ten_ky_thi { get; set; }
    public DateOnly ngay_thi { get; set; }
    public string? mo_ta { get; set; }
    public string trang_thai { get; set; }
}
