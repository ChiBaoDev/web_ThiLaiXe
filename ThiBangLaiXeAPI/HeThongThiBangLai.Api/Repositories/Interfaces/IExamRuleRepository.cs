using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface IExamRuleRepository
{
    Task<List<nhat_ky_he_thong>> GetRuleLogsAsync();
    Task<int> CountApprovedQuestionsByTopicAsync(long topicId);
    Task<int> CountApprovedQuestionsByDifficultyAsync(string difficulty);
    Task<int> CountApprovedCriticalQuestionsAsync();
    Task AddSystemLogAsync(nhat_ky_he_thong log);
    Task SaveChangesAsync();
}
