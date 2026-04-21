namespace HeThongThiBangLai.Api.DTOs;

public class buoi_hoc_dto
{
    public long id { get; set; }
    public long lop_hoc_id { get; set; }
    public string ten_buoi { get; set; }
    public DateOnly ngay_hoc { get; set; }
    public TimeOnly gio_bat_dau { get; set; }
    public TimeOnly gio_ket_thuc { get; set; }
    public string? noi_dung { get; set; }
    public string? phong_hoc { get; set; }
}
