namespace HeThongThiBangLai.Api.DTOs.Auth;

public class JwtTokenRequestDto
{
    public long user_id { get; set; }
    public string username { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public List<string> roles { get; set; } = new();
}
