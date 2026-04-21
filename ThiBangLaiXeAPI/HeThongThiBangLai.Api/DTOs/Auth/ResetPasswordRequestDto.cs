namespace HeThongThiBangLai.Api.DTOs.Auth;

public class ResetPasswordRequestDto
{
    public string email { get; set; } = string.Empty;
    public string reset_token { get; set; } = string.Empty;
    public string mat_khau_moi { get; set; } = string.Empty;
}
