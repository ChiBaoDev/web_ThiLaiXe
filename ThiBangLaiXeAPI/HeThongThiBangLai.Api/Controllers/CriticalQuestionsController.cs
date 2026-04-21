using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.CriticalQuestions;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/critical-questions")]
[Authorize]
[Produces("application/json")]
public class CriticalQuestionsController : ControllerBase
{
    private readonly ICriticalQuestionService _service;

    public CriticalQuestionsController(ICriticalQuestionService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CriticalQuestionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetList()
    {
        var result = await _service.GetListAsync();
        return Ok(result);
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<CriticalQuestionSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummary()
    {
        var userId = GetCurrentUserId();
        var result = await _service.GetSummaryAsync(userId);
        return Ok(result);
    }

    [HttpPost("start-practice")]
    [ProducesResponseType(typeof(ApiResponse<CriticalPracticeSessionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> StartPractice([FromBody] StartCriticalPracticeRequestDto request)
    {
        var userId = GetCurrentUserId();
        var result = await _service.StartPracticeAsync(userId, request);
        return StatusCode(StatusCodes.Status201Created, result);
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
