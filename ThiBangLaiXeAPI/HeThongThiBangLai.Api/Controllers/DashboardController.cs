using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Dashboard;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse<DashboardOverviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOverview()
    {
        var result = await _service.GetOverviewAsync();
        return Ok(result);
    }

    [HttpGet("exam-stats")]
    [ProducesResponseType(typeof(ApiResponse<DashboardExamStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExamStats([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var result = await _service.GetExamStatsAsync(from, to);
        return Ok(result);
    }

    [HttpGet("question-stats")]
    [ProducesResponseType(typeof(ApiResponse<DashboardQuestionStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetQuestionStats([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var result = await _service.GetQuestionStatsAsync(from, to);
        return Ok(result);
    }

    [HttpGet("weak-topics")]
    [ProducesResponseType(typeof(ApiResponse<List<DashboardWeakTopicDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetWeakTopics([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var result = await _service.GetWeakTopicsAsync(from, to);
        return Ok(result);
    }

    [HttpGet("critical-question-stats")]
    [ProducesResponseType(typeof(ApiResponse<DashboardCriticalQuestionStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCriticalQuestionStats([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var result = await _service.GetCriticalQuestionStatsAsync(from, to);
        return Ok(result);
    }
}
