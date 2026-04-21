using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Dashboard;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface IDashboardService
{
    Task<ApiResponse<DashboardOverviewDto>> GetOverviewAsync();
    Task<ApiResponse<DashboardExamStatsDto>> GetExamStatsAsync(DateTime? from = null, DateTime? to = null);
    Task<ApiResponse<DashboardQuestionStatsDto>> GetQuestionStatsAsync(DateTime? from = null, DateTime? to = null);
    Task<ApiResponse<List<DashboardWeakTopicDto>>> GetWeakTopicsAsync(DateTime? from = null, DateTime? to = null);
    Task<ApiResponse<DashboardCriticalQuestionStatsDto>> GetCriticalQuestionStatsAsync(DateTime? from = null, DateTime? to = null);
}
