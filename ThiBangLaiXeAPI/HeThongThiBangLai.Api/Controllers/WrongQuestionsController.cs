using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.WrongQuestions;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/wrong-questions")]
[Authorize]
[Produces("application/json")]
public class WrongQuestionsController : ControllerBase
{
    private readonly IWrongQuestionService _service;

    public WrongQuestionsController(IWrongQuestionService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<WrongQuestionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetList()
    {
        var userId = GetCurrentUserId();
        var result = await _service.GetListAsync(userId);
        return Ok(result);
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<WrongQuestionSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummary()
    {
        var userId = GetCurrentUserId();
        var result = await _service.GetSummaryAsync(userId);
        return Ok(result);
    }

    [HttpPost("start-practice")]
    [ProducesResponseType(typeof(ApiResponse<WrongPracticeSessionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> StartPractice([FromBody] StartWrongPracticeRequestDto request)
    {
        var userId = GetCurrentUserId();
        var result = await _service.StartPracticeAsync(userId, request);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPatch("{questionId}/resolved")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resolve(long questionId)
    {
        var userId = GetCurrentUserId();
        var result = await _service.ResolveAsync(userId, questionId);
        return Ok(result);
    }

    [HttpDelete("{questionId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long questionId)
    {
        var userId = GetCurrentUserId();
        var result = await _service.DeleteAsync(userId, questionId);
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
