using HeThongThiBangLai.Api.DTOs.Auth;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface IJwtTokenService
{
    JwtTokenResultDto GenerateToken(JwtTokenRequestDto request);
}
