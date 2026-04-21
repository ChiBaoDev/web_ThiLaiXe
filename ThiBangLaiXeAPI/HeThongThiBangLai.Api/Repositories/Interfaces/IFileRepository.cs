using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface IFileRepository
{
    Task<files?> GetByIdAsync(long id);
    Task<PagedList<files>> GetPagedAsync(int page, int pageSize, string? search = null, string? status = null);
    Task AddAsync(files file);
    void Update(files file);
    void Remove(files file);
    Task<List<file_usages>> GetUsagesByFileIdAsync(long fileId);
    Task<bool> ExistsUsageAsync(long fileId, string entityName, long entityId, string fieldName);
    Task AddUsageAsync(file_usages usage);
    Task SaveChangesAsync();
}
