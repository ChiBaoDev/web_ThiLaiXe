using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Dashboard;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface IDashboardRepository
{
    Task<int> GetTotalCandidatesAsync();
    Task<List<bai_thi>> GetExamSessionsAsync(DateTime? from = null, DateTime? to = null);
    Task<List<chi_tiet_bai_thi>> GetExamDetailsAsync(DateTime? from = null, DateTime? to = null);
    Task<List<TopicWeakStat>> GetWeakTopicStatsAsync(DateTime? from = null, DateTime? to = null);
    Task<List<QuestionWrongStat>> GetTopWrongQuestionsAsync(DateTime? from = null, DateTime? to = null, int take = 10, bool criticalOnly = false);
}
