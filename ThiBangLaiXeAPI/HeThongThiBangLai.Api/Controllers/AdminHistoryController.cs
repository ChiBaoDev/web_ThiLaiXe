using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.History;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/admin/history")]
[Authorize]
[Produces("application/json")]
public class AdminHistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public AdminHistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet("exams")]
    [ProducesResponseType(typeof(ApiResponse<PagedList<ExamHistoryItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExams([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] string? result = null)
    {
        var response = await _historyService.GetAdminExamHistoryAsync(page, pageSize, from, to, result);
        return Ok(response);
    }

    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<PagedList<ExamHistoryItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserExams(long userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] string? result = null)
    {
        var response = await _historyService.GetAdminUserExamHistoryAsync(userId, page, pageSize, from, to, result);
        return Ok(response);
    }
}
