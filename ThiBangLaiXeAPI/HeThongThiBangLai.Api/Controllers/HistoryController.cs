using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.History;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/history")]
[Authorize]
[Produces("application/json")]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet("exams")]
    [ProducesResponseType(typeof(ApiResponse<PagedList<ExamHistoryItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExams([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] string? result = null)
    {
        var userId = GetCurrentUserId();
        var response = await _historyService.GetCandidateExamHistoryAsync(userId, page, pageSize, from, to, result);
        return Ok(response);
    }

    [HttpGet("exams/{sessionId}")]
    [ProducesResponseType(typeof(ApiResponse<ExamHistoryDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExamDetail(long sessionId)
    {
        var userId = GetCurrentUserId();
        var response = await _historyService.GetCandidateExamHistoryDetailAsync(userId, sessionId);
        return Ok(response);
    }

    [HttpGet("analytics")]
    [ProducesResponseType(typeof(ApiResponse<ExamHistoryAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnalytics([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var userId = GetCurrentUserId();
        var response = await _historyService.GetCandidateAnalyticsAsync(userId, from, to);
        return Ok(response);
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
