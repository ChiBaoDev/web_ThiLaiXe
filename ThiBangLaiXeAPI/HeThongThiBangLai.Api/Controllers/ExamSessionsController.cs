using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.ExamSessions;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/exams")]
[Authorize]
[Produces("application/json")]
public class ExamSessionsController : ControllerBase
{
    private readonly IExamSessionService _service;

    public ExamSessionsController(IExamSessionService service)
    {
        _service = service;
    }

    [HttpPost("sample/{sampleExamId}/start")]
    [ProducesResponseType(typeof(ApiResponse<StartExamSessionResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> StartSampleExam(long sampleExamId)
    {
        var userId = GetCurrentUserId();
        var result = await _service.StartSampleExamAsync(userId, sampleExamId);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("sessions/{sessionId}")]
    [ProducesResponseType(typeof(ApiResponse<ExamSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSession(long sessionId)
    {
        var userId = GetCurrentUserId();
        var result = await _service.GetSessionAsync(userId, sessionId);
        return Ok(result);
    }

    [HttpGet("sessions/{sessionId}/questions/{number}")]
    [ProducesResponseType(typeof(ApiResponse<ExamSessionQuestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQuestion(long sessionId, int number)
    {
        var userId = GetCurrentUserId();
        var result = await _service.GetQuestionAsync(userId, sessionId, number);
        return Ok(result);
    }

    [HttpPost("sessions/{sessionId}/answers")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SubmitAnswer(long sessionId, [FromBody] SubmitExamAnswerRequestDto request)
    {
        var userId = GetCurrentUserId();
        var result = await _service.SubmitAnswerAsync(userId, sessionId, request);
        return Ok(result);
    }

    [HttpPost("sessions/{sessionId}/submit")]
    [ProducesResponseType(typeof(ApiResponse<ExamSessionResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Submit(long sessionId)
    {
        var userId = GetCurrentUserId();
        var result = await _service.SubmitAsync(userId, sessionId, false);
        return Ok(result);
    }

    [HttpPost("sessions/{sessionId}/auto-submit")]
    [ProducesResponseType(typeof(ApiResponse<ExamSessionResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AutoSubmit(long sessionId)
    {
        var userId = GetCurrentUserId();
        var result = await _service.SubmitAsync(userId, sessionId, true);
        return Ok(result);
    }

    [HttpGet("sessions/{sessionId}/result")]
    [ProducesResponseType(typeof(ApiResponse<ExamSessionResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetResult(long sessionId)
    {
        var userId = GetCurrentUserId();
        var result = await _service.GetResultAsync(userId, sessionId);
        return Ok(result);
    }

    [HttpGet("sessions/{sessionId}/review")]
    [ProducesResponseType(typeof(ApiResponse<ExamSessionReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetReview(long sessionId)
    {
        var userId = GetCurrentUserId();
        var result = await _service.GetReviewAsync(userId, sessionId);
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
