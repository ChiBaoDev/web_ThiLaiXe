using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.WrongQuestions;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface IWrongQuestionRepository
{
    Task<hoc_vien?> GetStudentByUserIdAsync(long userId);
    Task<List<WrongQuestionStat>> GetWrongQuestionStatsAsync(long hocVienId);
    Task<List<cau_hoi>> GetQuestionsByIdsAsync(IEnumerable<long> questionIds);
    Task<HashSet<long>> GetHandledQuestionIdsAsync(long userId);

    Task<int> GetWrongPracticeSessionCountAsync(long hocVienId);
    Task<DateTime?> GetLatestWrongPracticeAtAsync(long hocVienId);

    Task AddPracticeSessionAsync(phien_on_tap session);
    Task AddPracticeSessionQuestionsAsync(IEnumerable<phien_on_tap_cau_hoi> questions);

    Task AddSystemLogAsync(nhat_ky_he_thong log);
    Task SaveChangesAsync();
}
