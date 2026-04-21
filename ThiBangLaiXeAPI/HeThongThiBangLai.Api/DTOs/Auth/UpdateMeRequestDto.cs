namespace HeThongThiBangLai.Api.DTOs.Auth;

public class UpdateMeRequestDto
{
    public string? email { get; set; }
    public string? so_dien_thoai { get; set; }
    public string? ho_ten { get; set; }
    public DateOnly? ngay_sinh { get; set; }
    public string? gioi_tinh { get; set; }
    public string? cccd { get; set; }
    public string? dia_chi { get; set; }
    public string? anh_chan_dung { get; set; }
}
