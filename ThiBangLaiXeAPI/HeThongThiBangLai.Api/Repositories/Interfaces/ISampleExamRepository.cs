using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface ISampleExamRepository
{
    Task<de_thi?> GetByIdAsync(long id);
    Task<de_thi?> GetByCodeAsync(string code);
    Task<PagedList<de_thi>> GetPagedAsync(int page, int pageSize, string? search = null);

    Task<ky_thi?> GetExamPeriodByIdAsync(long id);
    Task<List<cau_hoi>> GetQuestionsByIdsAsync(List<long> ids);
    Task<de_thi_cau_hoi?> GetAssignmentAsync(long sampleExamId, long questionId);
    Task<int> GetMaxQuestionOrderAsync(long sampleExamId);

    Task AddAsync(de_thi sampleExam);
    void Update(de_thi sampleExam);
    void Remove(de_thi sampleExam);

    Task AddAssignmentAsync(de_thi_cau_hoi assignment);
    void RemoveAssignment(de_thi_cau_hoi assignment);
    void RemoveAssignments(IEnumerable<de_thi_cau_hoi> assignments);

    Task SaveChangesAsync();
}
