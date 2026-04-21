using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Cms;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface ICmsService
{
    Task<ApiResponse<PagedList<CategoryDto>>> GetCategoriesAsync(int page = 1, int pageSize = 20, string? search = null, bool? isActive = null);
    Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(long id);
    Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequestDto request, long? createdBy = null);
    Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(long id, UpdateCategoryRequestDto request);
    Task DeleteCategoryAsync(long id);

    Task<ApiResponse<PagedList<PostListResponseDto>>> GetPostsAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null, string? postType = null, bool publishedOnly = false);
    Task<ApiResponse<PostDto>> GetPostByIdAsync(long id, bool publishedOnly = false);
    Task<ApiResponse<PostDto>> CreatePostAsync(CreatePostRequestDto request, long? authorId = null);
    Task<ApiResponse<PostDto>> UpdatePostAsync(long id, UpdatePostRequestDto request);
    Task DeletePostAsync(long id);
}
