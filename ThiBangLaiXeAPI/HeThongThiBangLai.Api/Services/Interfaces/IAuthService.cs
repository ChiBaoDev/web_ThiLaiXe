using HeThongThiBangLai.Api.DTOs.Auth;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, string? ipAddress = null);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress = null);
    Task LogoutAsync(long userId, string? ipAddress = null);
    Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, string? ipAddress = null);
    Task ResetPasswordAsync(ResetPasswordRequestDto request, string? ipAddress = null);
    Task ChangePasswordAsync(long userId, ChangePasswordRequestDto request, string? ipAddress = null);
    Task<MeResponseDto> GetCurrentUserProfileAsync(long userId);
    Task<MeResponseDto> UpdateCurrentUserProfileAsync(long userId, UpdateMeRequestDto request, string? ipAddress = null);
}
