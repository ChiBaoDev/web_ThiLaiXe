using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Entitlements;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/entitlements")]
[Authorize]
[Produces("application/json")]
public class EntitlementsController : ControllerBase
{
    private readonly IEntitlementService _service;

    public EntitlementsController(IEntitlementService service)
    {
        _service = service;
    }

    [HttpGet("packages")]
    [ProducesResponseType(typeof(ApiResponse<PagedList<EntitlementPackageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPackages([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] bool? isActive = null)
    {
        var result = await _service.GetPackagesAsync(page, pageSize, search, isActive);
        return Ok(result);
    }

    [HttpGet("packages/{id}")]
    [ProducesResponseType(typeof(ApiResponse<EntitlementPackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPackageById(long id)
    {
        var result = await _service.GetPackageByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("packages")]
    [ProducesResponseType(typeof(ApiResponse<EntitlementPackageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePackage([FromBody] CreateEntitlementPackageRequestDto request)
    {
        var result = await _service.CreatePackageAsync(request);
        return CreatedAtAction(nameof(GetPackageById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("packages/{id}")]
    [ProducesResponseType(typeof(ApiResponse<EntitlementPackageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePackage(long id, [FromBody] UpdateEntitlementPackageRequestDto request)
    {
        var result = await _service.UpdatePackageAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("packages/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePackage(long id)
    {
        await _service.DeletePackageAsync(id);
        return NoContent();
    }

    [HttpGet("user-entitlements")]
    [ProducesResponseType(typeof(ApiResponse<PagedList<UserEntitlementDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserEntitlements([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] long? userId = null, [FromQuery] string? status = null)
    {
        var result = await _service.GetUserEntitlementsAsync(page, pageSize, userId, status);
        return Ok(result);
    }

    [HttpGet("user-entitlements/{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserEntitlementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserEntitlementById(long id)
    {
        var result = await _service.GetUserEntitlementByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("user-entitlements/grant")]
    [ProducesResponseType(typeof(ApiResponse<UserEntitlementDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> GrantUserEntitlement([FromBody] GrantUserEntitlementRequestDto request)
    {
        var result = await _service.GrantUserEntitlementAsync(request, GetCurrentUserId());
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPatch("user-entitlements/{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<UserEntitlementDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserEntitlementStatus(long id, [FromBody] UpdateUserEntitlementStatusRequestDto request)
    {
        var result = await _service.UpdateUserEntitlementStatusAsync(id, request);
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
