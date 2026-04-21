namespace HeThongThiBangLai.Api.DTOs.Auth;

public class JwtTokenResultDto
{
    public string token { get; set; } = string.Empty;
    public DateTime expires_at_utc { get; set; }
}
