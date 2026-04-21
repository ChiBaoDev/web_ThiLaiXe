using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Exams;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/sample-exams")]
[Authorize]
[Produces("application/json")]
public class SampleExamsController : ControllerBase
{
    private readonly ISampleExamService _sampleExamService;

    public SampleExamsController(ISampleExamService sampleExamService)
    {
        _sampleExamService = sampleExamService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedList<SampleExamDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        var result = await _sampleExamService.GetListAsync(page, pageSize, search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SampleExamDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sampleExamService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SampleExamDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSampleExamRequestDto request)
    {
        var result = await _sampleExamService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SampleExamDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateSampleExamRequestDto request)
    {
        var result = await _sampleExamService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpPost("{id}/questions")]
    [ProducesResponseType(typeof(ApiResponse<SampleExamDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AssignQuestions(long id, [FromBody] AssignSampleExamQuestionsRequestDto request)
    {
        var result = await _sampleExamService.AssignQuestionsAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}/questions/{questionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestion(long id, long questionId)
    {
        await _sampleExamService.DeleteQuestionAsync(id, questionId);
        return NoContent();
    }

    [HttpPatch("{id}/publish")]
    [ProducesResponseType(typeof(ApiResponse<SampleExamDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Publish(long id)
    {
        var result = await _sampleExamService.PublishAsync(id);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(long id)
    {
        await _sampleExamService.DeleteAsync(id);
        return NoContent();
    }
}
