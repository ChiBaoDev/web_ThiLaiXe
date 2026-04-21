using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HeThongThiBangLai.Api.DTOs.Auth;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HeThongThiBangLai.Api.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JwtTokenResultDto GenerateToken(JwtTokenRequestDto request)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Thiếu cấu hình Jwt:Issuer");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Thiếu cấu hình Jwt:Audience");
        var secret = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Thiếu cấu hình Jwt:SecretKey");
        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var minutes) ? minutes : 480;

        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.user_id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, request.username),
            new(JwtRegisteredClaimNames.Email, request.email)
        };

        claims.AddRange(request.roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: issuedAt,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtTokenResultDto
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expires_at_utc = expiresAt
        };
    }
}
