using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface IHistoryRepository
{
    Task<hoc_vien?> GetStudentByUserIdAsync(long userId);
    Task<bai_thi?> GetExamByIdAsync(long sessionId);
    Task<bai_thi?> GetExamByIdForStudentAsync(long sessionId, long hocVienId);
    Task<PagedList<bai_thi>> GetExamListForStudentAsync(long hocVienId, int page, int pageSize, DateTime? from = null, DateTime? to = null, string? result = null);
    Task<PagedList<bai_thi>> GetExamListForAdminAsync(int page, int pageSize, DateTime? from = null, DateTime? to = null, string? result = null);
    Task<PagedList<bai_thi>> GetExamListByStudentIdForAdminAsync(long hocVienId, int page, int pageSize, DateTime? from = null, DateTime? to = null, string? result = null);
}
