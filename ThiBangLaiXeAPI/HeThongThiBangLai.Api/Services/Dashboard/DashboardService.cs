using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Dashboard;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;

    public DashboardService(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<DashboardOverviewDto>> GetOverviewAsync()
    {
        var totalCandidates = await _repository.GetTotalCandidatesAsync();
        var sessions = await _repository.GetExamSessionsAsync();
        var examDetails = await _repository.GetExamDetailsAsync();

        var totalSessions = sessions.Count;
        var passed = sessions.Count(x => x.ket_qua == "pass");
        var passRate = totalSessions == 0 ? 0 : (decimal)passed * 100 / totalSessions;
        var averageScore = totalSessions == 0 ? 0 : sessions.Average(x => x.diem);

        var criticalFailedSessionIds = examDetails
            .Where(x => x.la_dung == false && x.cau_hoi.la_cau_diem_liet)
            .Select(x => x.bai_thi_id)
            .Distinct()
            .ToHashSet();

        var criticalFailRate = totalSessions == 0 ? 0 : (decimal)criticalFailedSessionIds.Count * 100 / totalSessions;

        var dto = new DashboardOverviewDto
        {
            TotalCandidates = totalCandidates,
            TotalSessions = totalSessions,
            PassRate = decimal.Round(passRate, 2),
            AverageScore = decimal.Round(averageScore, 2),
            CriticalFailRate = decimal.Round(criticalFailRate, 2)
        };

        return ApiResponseFactory.Success(dto, "Dashboard overview retrieved successfully");
    }

    public async Task<ApiResponse<DashboardExamStatsDto>> GetExamStatsAsync(DateTime? from = null, DateTime? to = null)
    {
        var sessions = await _repository.GetExamSessionsAsync(from, to);

        var totalSessions = sessions.Count;
        var passed = sessions.Count(x => x.ket_qua == "pass");
        var failed = sessions.Count(x => x.ket_qua == "fail");
        var passRate = totalSessions == 0 ? 0 : (decimal)passed * 100 / totalSessions;
        var averageScore = totalSessions == 0 ? 0 : sessions.Average(x => x.diem);

        var trend = sessions
            .Where(x => x.thoi_gian_nop.HasValue)
            .GroupBy(x => x.thoi_gian_nop!.Value.Date)
            .Select(g => new DashboardTrendPointDto
            {
                Date = g.Key,
                SessionCount = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToList();

        var dto = new DashboardExamStatsDto
        {
            From = from,
            To = to,
            TotalSessions = totalSessions,
            PassedSessions = passed,
            FailedSessions = failed,
            PassRate = decimal.Round(passRate, 2),
            AverageScore = decimal.Round(averageScore, 2),
            DailyTrend = trend
        };

        return ApiResponseFactory.Success(dto, "Dashboard exam stats retrieved successfully");
    }

    public async Task<ApiResponse<DashboardQuestionStatsDto>> GetQuestionStatsAsync(DateTime? from = null, DateTime? to = null)
    {
        var details = await _repository.GetExamDetailsAsync(from, to);

        var totalAnswered = details.Count(x => x.la_dung.HasValue);
        var correct = details.Count(x => x.la_dung == true);
        var wrong = details.Count(x => x.la_dung == false);
        var accuracyRate = totalAnswered == 0 ? 0 : (decimal)correct * 100 / totalAnswered;

        var topWrongQuestions = await _repository.GetTopWrongQuestionsAsync(from, to, 10);

        var dto = new DashboardQuestionStatsDto
        {
            TotalAnsweredQuestions = totalAnswered,
            CorrectAnswers = correct,
            WrongAnswers = wrong,
            AccuracyRate = decimal.Round(accuracyRate, 2),
            MostWrongQuestions = topWrongQuestions
                .Select(x => new DashboardQuestionErrorDto
                {
                    QuestionId = x.QuestionId,
                    QuestionContent = x.QuestionContent,
                    WrongCount = x.WrongCount
                })
                .ToList()
        };

        return ApiResponseFactory.Success(dto, "Dashboard question stats retrieved successfully");
    }

    public async Task<ApiResponse<List<DashboardWeakTopicDto>>> GetWeakTopicsAsync(DateTime? from = null, DateTime? to = null)
    {
        var stats = await _repository.GetWeakTopicStatsAsync(from, to);

        var dto = stats
            .Select(x =>
            {
                var accuracy = x.TotalAnswered == 0 ? 0 : (decimal)(x.TotalAnswered - x.WrongCount) * 100 / x.TotalAnswered;
                return new DashboardWeakTopicDto
                {
                    TopicId = x.TopicId,
                    TopicName = x.TopicName,
                    TotalAnswered = x.TotalAnswered,
                    WrongCount = x.WrongCount,
                    AccuracyRate = decimal.Round(accuracy, 2)
                };
            })
            .OrderBy(x => x.AccuracyRate)
            .ThenByDescending(x => x.WrongCount)
            .Take(10)
            .ToList();

        return ApiResponseFactory.Success(dto, "Dashboard weak topics retrieved successfully");
    }

    public async Task<ApiResponse<DashboardCriticalQuestionStatsDto>> GetCriticalQuestionStatsAsync(DateTime? from = null, DateTime? to = null)
    {
        var details = await _repository.GetExamDetailsAsync(from, to);

        var criticalDetails = details.Where(x => x.cau_hoi.la_cau_diem_liet).ToList();
        var totalCriticalAttempts = criticalDetails.Count(x => x.la_dung.HasValue);
        var wrongCriticalAttempts = criticalDetails.Count(x => x.la_dung == false);
        var criticalErrorRate = totalCriticalAttempts == 0 ? 0 : (decimal)wrongCriticalAttempts * 100 / totalCriticalAttempts;

        var topCriticalWrong = await _repository.GetTopWrongQuestionsAsync(from, to, 10, true);

        var dto = new DashboardCriticalQuestionStatsDto
        {
            TotalCriticalAttempts = totalCriticalAttempts,
            WrongCriticalAttempts = wrongCriticalAttempts,
            CriticalErrorRate = decimal.Round(criticalErrorRate, 2),
            TopCriticalWrongQuestions = topCriticalWrong
                .Select(x => new DashboardQuestionErrorDto
                {
                    QuestionId = x.QuestionId,
                    QuestionContent = x.QuestionContent,
                    WrongCount = x.WrongCount
                })
                .ToList()
        };

        return ApiResponseFactory.Success(dto, "Dashboard critical question stats retrieved successfully");
    }
}
