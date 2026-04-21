using AutoMapper;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Cms;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.Cms;

public class CmsService : ICmsService
{
    private readonly ICmsRepository _repository;
    private readonly IMapper _mapper;

    public CmsService(ICmsRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedList<CategoryDto>>> GetCategoriesAsync(int page = 1, int pageSize = 20, string? search = null, bool? isActive = null)
    {
        var paged = await _repository.GetCategoriesPagedAsync(page, pageSize, search, isActive);
        var dtos = _mapper.Map<List<CategoryDto>>(paged.Items);
        var result = new PagedList<CategoryDto>(dtos, paged.TotalCount, page, pageSize);

        return ApiResponseFactory.SuccessPaged(result, "Categories retrieved successfully");
    }

    public async Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(long id)
    {
        var entity = await _repository.GetCategoryByIdAsync(id);
        if (entity == null)
            return ApiResponseFactory.Fail<CategoryDto>("Category not found");

        var dto = _mapper.Map<CategoryDto>(entity);
        return ApiResponseFactory.Success(dto, "Category retrieved successfully");
    }

    public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequestDto request, long? createdBy = null)
    {
        if (request.ParentId.HasValue)
        {
            var parent = await _repository.GetCategoryByIdAsync(request.ParentId.Value);
            if (parent == null)
            {
                throw new NotFoundAppException("Parent category not found");
            }
        }

        var existingCode = await _repository.GetCategoryByCodeAsync(request.Code);
        if (existingCode != null)
        {
            throw new ConflictAppException("Category code already exists", "CATEGORY_CODE_EXISTS");
        }

        var existingSlug = await _repository.GetCategoryBySlugAsync(request.Slug);
        if (existingSlug != null)
        {
            throw new ConflictAppException("Category slug already exists", "CATEGORY_SLUG_EXISTS");
        }

        var entity = _mapper.Map<categories>(request);
        entity.created_by = createdBy;

        await _repository.AddCategoryAsync(entity);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<CategoryDto>(entity);
        return ApiResponseFactory.Created(dto, "Category created successfully");
    }

    public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(long id, UpdateCategoryRequestDto request)
    {
        var entity = await _repository.GetCategoryByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundAppException("Category not found");
        }

        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == id)
            {
                throw new BusinessRuleAppException("Parent category cannot be itself", "INVALID_PARENT_CATEGORY");
            }

            var parent = await _repository.GetCategoryByIdAsync(request.ParentId.Value);
            if (parent == null)
            {
                throw new NotFoundAppException("Parent category not found");
            }
        }

        var existingCode = await _repository.GetCategoryByCodeAsync(request.Code);
        if (existingCode != null && existingCode.id != id)
        {
            throw new ConflictAppException("Category code already exists", "CATEGORY_CODE_EXISTS");
        }

        var existingSlug = await _repository.GetCategoryBySlugAsync(request.Slug);
        if (existingSlug != null && existingSlug.id != id)
        {
            throw new ConflictAppException("Category slug already exists", "CATEGORY_SLUG_EXISTS");
        }

        _mapper.Map(request, entity);
        entity.updated_at = DateTime.UtcNow;

        _repository.UpdateCategory(entity);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<CategoryDto>(entity);
        return ApiResponseFactory.Success(dto, "Category updated successfully");
    }

    public async Task DeleteCategoryAsync(long id)
    {
        var entity = await _repository.GetCategoryByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundAppException("Category not found");
        }

        var hasPosts = await _repository.CategoryHasPostsAsync(id);
        if (hasPosts)
        {
            throw new ConflictAppException("Cannot delete category with linked posts", "CATEGORY_HAS_POSTS");
        }

        _repository.RemoveCategory(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task<ApiResponse<PagedList<PostListResponseDto>>> GetPostsAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null, string? postType = null, bool publishedOnly = false)
    {
        var paged = await _repository.GetPostsPagedAsync(page, pageSize, search, status, postType, publishedOnly);
        var dtos = _mapper.Map<List<PostListResponseDto>>(paged.Items);
        var result = new PagedList<PostListResponseDto>(dtos, paged.TotalCount, page, pageSize);

        return ApiResponseFactory.SuccessPaged(result, "Posts retrieved successfully");
    }

    public async Task<ApiResponse<PostDto>> GetPostByIdAsync(long id, bool publishedOnly = false)
    {
        var entity = await _repository.GetPostByIdAsync(id);
        if (entity == null)
            return ApiResponseFactory.Fail<PostDto>("Post not found");

        if (publishedOnly)
        {
            var now = DateTime.UtcNow;
            if (entity.trang_thai != "published" || (entity.published_at.HasValue && entity.published_at > now))
            {
                return ApiResponseFactory.Fail<PostDto>("Post not found");
            }
        }

        var dto = _mapper.Map<PostDto>(entity);
        return ApiResponseFactory.Success(dto, "Post retrieved successfully");
    }

    public async Task<ApiResponse<PostDto>> CreatePostAsync(CreatePostRequestDto request, long? authorId = null)
    {
        var existingCode = await _repository.GetPostByCodeAsync(request.Code);
        if (existingCode != null)
        {
            throw new ConflictAppException("Post code already exists", "POST_CODE_EXISTS");
        }

        var existingSlug = await _repository.GetPostBySlugAsync(request.Slug);
        if (existingSlug != null)
        {
            throw new ConflictAppException("Post slug already exists", "POST_SLUG_EXISTS");
        }

        var distinctCategoryIds = request.CategoryIds.Distinct().ToList();
        var allExist = await _repository.AllCategoriesExistAsync(distinctCategoryIds);
        if (!allExist)
        {
            throw new NotFoundAppException("One or more categories not found");
        }

        var entity = _mapper.Map<posts>(request);
        entity.author_id = authorId;

        if (entity.trang_thai == "published" && !entity.published_at.HasValue)
        {
            entity.published_at = DateTime.UtcNow;
        }

        await _repository.AddPostAsync(entity);
        await _repository.SaveChangesAsync();

        var links = distinctCategoryIds
            .Select(categoryId => new post_categories
            {
                post_id = entity.id,
                category_id = categoryId
            })
            .ToList();

        await _repository.AddPostCategoriesAsync(links);
        await _repository.SaveChangesAsync();

        var created = await _repository.GetPostByIdAsync(entity.id) ?? entity;
        var dto = _mapper.Map<PostDto>(created);

        return ApiResponseFactory.Created(dto, "Post created successfully");
    }

    public async Task<ApiResponse<PostDto>> UpdatePostAsync(long id, UpdatePostRequestDto request)
    {
        var entity = await _repository.GetPostByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundAppException("Post not found");
        }

        var existingCode = await _repository.GetPostByCodeAsync(request.Code);
        if (existingCode != null && existingCode.id != id)
        {
            throw new ConflictAppException("Post code already exists", "POST_CODE_EXISTS");
        }

        var existingSlug = await _repository.GetPostBySlugAsync(request.Slug);
        if (existingSlug != null && existingSlug.id != id)
        {
            throw new ConflictAppException("Post slug already exists", "POST_SLUG_EXISTS");
        }

        var distinctCategoryIds = request.CategoryIds.Distinct().ToList();
        var allExist = await _repository.AllCategoriesExistAsync(distinctCategoryIds);
        if (!allExist)
        {
            throw new NotFoundAppException("One or more categories not found");
        }

        _mapper.Map(request, entity);
        if (entity.trang_thai == "published" && !entity.published_at.HasValue)
        {
            entity.published_at = DateTime.UtcNow;
        }

        entity.updated_at = DateTime.UtcNow;

        _repository.UpdatePost(entity);
        await _repository.SaveChangesAsync();

        await _repository.RemovePostCategoriesAsync(id);

        var links = distinctCategoryIds
            .Select(categoryId => new post_categories
            {
                post_id = id,
                category_id = categoryId
            })
            .ToList();

        await _repository.AddPostCategoriesAsync(links);
        await _repository.SaveChangesAsync();

        var updated = await _repository.GetPostByIdAsync(id) ?? entity;
        var dto = _mapper.Map<PostDto>(updated);

        return ApiResponseFactory.Success(dto, "Post updated successfully");
    }

    public async Task DeletePostAsync(long id)
    {
        var entity = await _repository.GetPostByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundAppException("Post not found");
        }

        await _repository.RemovePostCategoriesAsync(id);
        _repository.RemovePost(entity);
        await _repository.SaveChangesAsync();
    }
}
