using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.Cms;

public class CmsRepository : ICmsRepository
{
    private readonly ApplicationDbContext _context;

    public CmsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<categories?> GetCategoryByIdAsync(long id)
    {
        return await _context.categories.FindAsync(id);
    }

    public async Task<categories?> GetCategoryByCodeAsync(string code)
    {
        return await _context.categories.FirstOrDefaultAsync(x => x.ma_danh_muc == code);
    }

    public async Task<categories?> GetCategoryBySlugAsync(string slug)
    {
        return await _context.categories.FirstOrDefaultAsync(x => x.slug == slug);
    }

    public async Task<PagedList<categories>> GetCategoriesPagedAsync(int page, int pageSize, string? search = null, bool? isActive = null)
    {
        var query = _context.categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.ten_danh_muc.Contains(search)
                || x.ma_danh_muc.Contains(search)
                || x.slug.Contains(search));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.is_active == isActive.Value);
        }

        query = query.OrderByDescending(x => x.created_at);

        return await PagedList<categories>.CreateAsync(query, page, pageSize);
    }

    public async Task AddCategoryAsync(categories category)
    {
        await _context.categories.AddAsync(category);
    }

    public void UpdateCategory(categories category)
    {
        _context.categories.Update(category);
    }

    public void RemoveCategory(categories category)
    {
        _context.categories.Remove(category);
    }

    public async Task<bool> CategoryHasPostsAsync(long categoryId)
    {
        return await _context.post_categories.AnyAsync(x => x.category_id == categoryId);
    }

    public async Task<posts?> GetPostByIdAsync(long id)
    {
        return await _context.posts
            .Include(x => x.post_categories)
            .FirstOrDefaultAsync(x => x.id == id);
    }

    public async Task<posts?> GetPostByCodeAsync(string code)
    {
        return await _context.posts.FirstOrDefaultAsync(x => x.ma_bai_viet == code);
    }

    public async Task<posts?> GetPostBySlugAsync(string slug)
    {
        return await _context.posts.FirstOrDefaultAsync(x => x.slug == slug);
    }

    public async Task<PagedList<posts>> GetPostsPagedAsync(int page, int pageSize, string? search = null, string? status = null, string? postType = null, bool publishedOnly = false)
    {
        var query = _context.posts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.title.Contains(search)
                || x.ma_bai_viet.Contains(search)
                || x.slug.Contains(search)
                || (x.summary != null && x.summary.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(postType))
        {
            query = query.Where(x => x.post_type == postType);
        }

        if (publishedOnly)
        {
            var now = DateTime.UtcNow;
            query = query.Where(x => x.trang_thai == "published" && (!x.published_at.HasValue || x.published_at <= now));
        }
        else if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.trang_thai == status);
        }

        query = query.OrderByDescending(x => x.published_at ?? x.created_at);

        return await PagedList<posts>.CreateAsync(query, page, pageSize);
    }

    public async Task AddPostAsync(posts post)
    {
        await _context.posts.AddAsync(post);
    }

    public void UpdatePost(posts post)
    {
        _context.posts.Update(post);
    }

    public void RemovePost(posts post)
    {
        _context.posts.Remove(post);
    }

    public async Task RemovePostCategoriesAsync(long postId)
    {
        var links = await _context.post_categories.Where(x => x.post_id == postId).ToListAsync();
        if (links.Count > 0)
        {
            _context.post_categories.RemoveRange(links);
        }
    }

    public async Task AddPostCategoriesAsync(List<post_categories> postCategories)
    {
        if (postCategories.Count > 0)
        {
            await _context.post_categories.AddRangeAsync(postCategories);
        }
    }

    public async Task<bool> AllCategoriesExistAsync(IEnumerable<long> categoryIds)
    {
        var ids = categoryIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return true;
        }

        var count = await _context.categories.CountAsync(x => ids.Contains(x.id));
        return count == ids.Count;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
