using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.CriticalQuestions;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface ICriticalQuestionService
{
    Task<ApiResponse<List<CriticalQuestionDto>>> GetListAsync();
    Task<ApiResponse<CriticalQuestionSummaryDto>> GetSummaryAsync(long userId);
    Task<ApiResponse<CriticalPracticeSessionDto>> StartPracticeAsync(long userId, StartCriticalPracticeRequestDto request);
}
