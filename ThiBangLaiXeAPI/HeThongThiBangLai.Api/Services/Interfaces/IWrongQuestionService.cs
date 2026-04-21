using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.WrongQuestions;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface IWrongQuestionService
{
    Task<ApiResponse<List<WrongQuestionDto>>> GetListAsync(long userId);
    Task<ApiResponse<WrongQuestionSummaryDto>> GetSummaryAsync(long userId);
    Task<ApiResponse<WrongPracticeSessionDto>> StartPracticeAsync(long userId, StartWrongPracticeRequestDto request);
    Task<ApiResponse<object>> ResolveAsync(long userId, long questionId);
    Task<ApiResponse<object>> DeleteAsync(long userId, long questionId);
}
