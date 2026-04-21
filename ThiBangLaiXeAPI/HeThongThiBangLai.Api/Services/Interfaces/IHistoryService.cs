using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.History;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface IHistoryService
{
    Task<ApiResponse<PagedList<ExamHistoryItemDto>>> GetCandidateExamHistoryAsync(long userId, int page = 1, int pageSize = 20, DateTime? from = null, DateTime? to = null, string? result = null);
    Task<ApiResponse<ExamHistoryDetailDto>> GetCandidateExamHistoryDetailAsync(long userId, long sessionId);
    Task<ApiResponse<ExamHistoryAnalyticsDto>> GetCandidateAnalyticsAsync(long userId, DateTime? from = null, DateTime? to = null);

    Task<ApiResponse<PagedList<ExamHistoryItemDto>>> GetAdminExamHistoryAsync(int page = 1, int pageSize = 20, DateTime? from = null, DateTime? to = null, string? result = null);
    Task<ApiResponse<PagedList<ExamHistoryItemDto>>> GetAdminUserExamHistoryAsync(long userId, int page = 1, int pageSize = 20, DateTime? from = null, DateTime? to = null, string? result = null);
}
