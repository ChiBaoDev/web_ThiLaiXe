using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.Files;

public class FileRepository : IFileRepository
{
    private readonly ApplicationDbContext _context;

    public FileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<files?> GetByIdAsync(long id)
    {
        return await _context.files.FindAsync(id);
    }

    public async Task<PagedList<files>> GetPagedAsync(int page, int pageSize, string? search = null, string? status = null)
    {
        var query = _context.files.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.file_name.Contains(search)
                || x.object_key.Contains(search)
                || x.public_url.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.trang_thai == status);
        }

        query = query.OrderByDescending(x => x.created_at);

        return await PagedList<files>.CreateAsync(query, page, pageSize);
    }

    public async Task AddAsync(files file)
    {
        await _context.files.AddAsync(file);
    }

    public void Update(files file)
    {
        _context.files.Update(file);
    }

    public void Remove(files file)
    {
        _context.files.Remove(file);
    }

    public async Task<List<file_usages>> GetUsagesByFileIdAsync(long fileId)
    {
        return await _context.file_usages
            .Where(x => x.file_id == fileId)
            .OrderByDescending(x => x.is_primary)
            .ThenBy(x => x.sort_order)
            .ThenBy(x => x.id)
            .ToListAsync();
    }

    public async Task<bool> ExistsUsageAsync(long fileId, string entityName, long entityId, string fieldName)
    {
        return await _context.file_usages.AnyAsync(x =>
            x.file_id == fileId
            && x.entity_name == entityName
            && x.entity_id == entityId
            && x.field_name == fieldName);
    }

    public async Task AddUsageAsync(file_usages usage)
    {
        await _context.file_usages.AddAsync(usage);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
