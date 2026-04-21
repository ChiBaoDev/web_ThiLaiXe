using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Certificates;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/certificates")]
[Authorize]
[Produces("application/json")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _service;

    public CertificatesController(ICertificateService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedList<CertificateDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] string? status = null)
    {
        var result = await _service.GetListAsync(page, pageSize, search, status);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("issue")]
    [Authorize(Policy = "CanIssueCertificate")]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Issue([FromBody] IssueCertificateRequestDto request)
    {
        var result = await _service.IssueAsync(request, GetCurrentUserId());
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "CanIssueCertificate")]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateCertificateStatusRequestDto request)
    {
        var result = await _service.UpdateStatusAsync(id, request);
        return Ok(result);
    }

    [HttpPost("exam-results/{examResultId}/confirm")]
    [Authorize(Policy = "CanIssueCertificate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmExamResult(long examResultId, [FromBody] ConfirmExamResultRequestDto request)
    {
        var result = await _service.ConfirmExamResultAsync(examResultId, request, GetCurrentUserId());
        return Ok(result);
    }

    private long GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!long.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc thiếu thông tin người dùng.");

        return userId;
    }
}
