namespace HeThongThiBangLai.Api.DTOs.Auth;

public class LoginResponseDto
{
    public long user_id { get; set; }
    public string ten_dang_nhap { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string access_token { get; set; } = string.Empty;
    public DateTime expires_at_utc { get; set; }
    public List<string> roles { get; set; } = new();
}
