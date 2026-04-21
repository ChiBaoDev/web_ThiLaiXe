namespace HeThongThiBangLai.Api.DTOs.Auth;

public class RegisterResponseDto
{
    public long user_id { get; set; }
    public string ten_dang_nhap { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string role_mac_dinh { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
}
