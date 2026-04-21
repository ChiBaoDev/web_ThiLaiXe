namespace HeThongThiBangLai.Api.DTOs;

public class nguoi_dung_dto
{
    public long id { get; set; }
    public string ten_dang_nhap { get; set; }
    public string mat_khau_hash { get; set; }
    public string email { get; set; }
    public string? so_dien_thoai { get; set; }
    public string trang_thai { get; set; }
    public DateTime? lan_dang_nhap_cuoi { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
