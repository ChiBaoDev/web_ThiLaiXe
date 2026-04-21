using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface IExamSessionRepository
{
    Task<hoc_vien?> GetStudentByUserIdAsync(long userId);
    Task<de_thi?> GetPublishedSampleExamByIdAsync(long sampleExamId);

    Task AddExamSessionAsync(bai_thi session);
    Task AddExamSessionDetailsAsync(IEnumerable<chi_tiet_bai_thi> details);
    void UpdateExamSession(bai_thi session);
    void UpdateExamSessionDetail(chi_tiet_bai_thi detail);

    Task<bai_thi?> GetSessionByIdForStudentAsync(long sessionId, long hocVienId);
    Task<List<chi_tiet_bai_thi>> GetSessionDetailsAsync(long sessionId);

    Task AddSystemLogAsync(nhat_ky_he_thong log);
    Task SaveChangesAsync();
}
