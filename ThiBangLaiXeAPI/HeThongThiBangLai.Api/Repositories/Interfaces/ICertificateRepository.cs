using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface ICertificateRepository
{
    Task<certificates?> GetByIdAsync(long id);
    Task<certificates?> GetByCodeAsync(string code);
    Task<certificates?> GetByExamResultIdAsync(long examResultId);
    Task<PagedList<certificates>> GetPagedAsync(int page, int pageSize, string? search = null, string? status = null);
    Task AddAsync(certificates certificate);
    void Update(certificates certificate);

    Task<exam_results?> GetExamResultByIdAsync(long id);
    Task<bool> StudentExistsAsync(long studentId);
    Task<bool> FileExistsAsync(long fileId);

    Task SaveChangesAsync();
}
