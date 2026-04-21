using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface ICmsRepository
{
    Task<categories?> GetCategoryByIdAsync(long id);
    Task<categories?> GetCategoryByCodeAsync(string code);
    Task<categories?> GetCategoryBySlugAsync(string slug);
    Task<PagedList<categories>> GetCategoriesPagedAsync(int page, int pageSize, string? search = null, bool? isActive = null);
    Task AddCategoryAsync(categories category);
    void UpdateCategory(categories category);
    void RemoveCategory(categories category);
    Task<bool> CategoryHasPostsAsync(long categoryId);

    Task<posts?> GetPostByIdAsync(long id);
    Task<posts?> GetPostByCodeAsync(string code);
    Task<posts?> GetPostBySlugAsync(string slug);
    Task<PagedList<posts>> GetPostsPagedAsync(int page, int pageSize, string? search = null, string? status = null, string? postType = null, bool publishedOnly = false);
    Task AddPostAsync(posts post);
    void UpdatePost(posts post);
    void RemovePost(posts post);

    Task RemovePostCategoriesAsync(long postId);
    Task AddPostCategoriesAsync(List<post_categories> postCategories);
    Task<bool> AllCategoriesExistAsync(IEnumerable<long> categoryIds);

    Task SaveChangesAsync();
}
