namespace webthibanglai.Models;

public class LoginViewModel
{
    public LoginRequestModel LoginRequest { get; set; } = new();
    public RegisterRequestModel RegisterRequest { get; set; } = new();
    public AuthTokenResponse? AuthToken { get; set; }
    public CurrentUserInfo? CurrentUser { get; set; }
}

public class LoginRequestModel
{
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
}

public class RegisterRequestModel
{
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public DateOnly NgaySinh { get; set; }
    public string GioiTinh { get; set; } = string.Empty;
    public string Cccd { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string AnhChanDung { get; set; } = string.Empty;
}

public class AuthTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public AuthUserInfo? User { get; set; }
}

public class AuthUserInfo
{
    public int UserId { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AnhChanDung { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CurrentUserInfo
{
    public int UserId { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public DateOnly NgaySinh { get; set; }
    public string GioiTinh { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string AnhChanDung { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
