namespace HeThongThiBangLai.Api.DTOs.Auth;

public class MeResponseDto
{
    public long user_id { get; set; }
    public string ten_dang_nhap { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? so_dien_thoai { get; set; }
    public string trang_thai { get; set; } = string.Empty;

    public long hoc_vien_id { get; set; }
    public string ho_ten { get; set; } = string.Empty;
    public DateOnly? ngay_sinh { get; set; }
    public string? gioi_tinh { get; set; }
    public string? cccd { get; set; }
    public string? dia_chi { get; set; }
    public string? anh_chan_dung { get; set; }
    public List<string> roles { get; set; } = new();
}
