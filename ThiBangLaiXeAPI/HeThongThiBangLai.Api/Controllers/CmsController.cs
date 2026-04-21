using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Cms;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/cms")]
[Authorize]
[Produces("application/json")]
public class CmsController : ControllerBase
{
    private readonly ICmsService _cmsService;

    public CmsController(ICmsService cmsService)
    {
        _cmsService = cmsService;
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponse<PagedList<CategoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] bool? isActive = null)
    {
        var result = await _cmsService.GetCategoriesAsync(page, pageSize, search, isActive);
        return Ok(result);
    }

    [HttpGet("categories/{id}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(long id)
    {
        var result = await _cmsService.GetCategoryByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("categories")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
    {
        var result = await _cmsService.CreateCategoryAsync(request, GetCurrentUserId());
        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("categories/{id}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCategory(long id, [FromBody] UpdateCategoryRequestDto request)
    {
        var result = await _cmsService.UpdateCategoryAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("categories/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCategory(long id)
    {
        await _cmsService.DeleteCategoryAsync(id);
        return NoContent();
    }

    [HttpGet("posts")]
    [ProducesResponseType(typeof(ApiResponse<PagedList<PostListResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] string? status = null, [FromQuery] string? postType = null)
    {
        var result = await _cmsService.GetPostsAsync(page, pageSize, search, status, postType, false);
        return Ok(result);
    }

    [HttpGet("posts/{id}")]
    [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById(long id)
    {
        var result = await _cmsService.GetPostByIdAsync(id, false);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("posts")]
    [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequestDto request)
    {
        var result = await _cmsService.CreatePostAsync(request, GetCurrentUserId());
        return CreatedAtAction(nameof(GetPostById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("posts/{id}")]
    [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdatePost(long id, [FromBody] UpdatePostRequestDto request)
    {
        var result = await _cmsService.UpdatePostAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("posts/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(long id)
    {
        await _cmsService.DeletePostAsync(id);
        return NoContent();
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
