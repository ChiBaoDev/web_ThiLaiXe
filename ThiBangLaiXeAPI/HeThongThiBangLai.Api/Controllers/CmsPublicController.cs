using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Cms;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/public/cms")]
[AllowAnonymous]
[Produces("application/json")]
public class CmsPublicController : ControllerBase
{
    private readonly ICmsService _cmsService;

    public CmsPublicController(ICmsService cmsService)
    {
        _cmsService = cmsService;
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponse<PagedList<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        var result = await _cmsService.GetCategoriesAsync(page, pageSize, search, true);
        return Ok(result);
    }

    [HttpGet("posts")]
    [ProducesResponseType(typeof(ApiResponse<PagedList<PostListResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] string? postType = null)
    {
        var result = await _cmsService.GetPostsAsync(page, pageSize, search, null, postType, true);
        return Ok(result);
    }

    [HttpGet("posts/{id}")]
    [ProducesResponseType(typeof(ApiResponse<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById(long id)
    {
        var result = await _cmsService.GetPostByIdAsync(id, true);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
