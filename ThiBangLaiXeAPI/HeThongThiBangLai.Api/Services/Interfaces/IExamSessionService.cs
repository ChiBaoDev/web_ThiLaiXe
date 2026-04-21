using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.ExamSessions;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface IExamSessionService
{
    Task<ApiResponse<StartExamSessionResponseDto>> StartSampleExamAsync(long userId, long sampleExamId);
    Task<ApiResponse<ExamSessionDto>> GetSessionAsync(long userId, long sessionId);
    Task<ApiResponse<ExamSessionQuestionDto>> GetQuestionAsync(long userId, long sessionId, int number);
    Task<ApiResponse<object>> SubmitAnswerAsync(long userId, long sessionId, SubmitExamAnswerRequestDto request);
    Task<ApiResponse<ExamSessionResultDto>> SubmitAsync(long userId, long sessionId, bool isAutoSubmit = false);
    Task<ApiResponse<ExamSessionResultDto>> GetResultAsync(long userId, long sessionId);
    Task<ApiResponse<ExamSessionReviewDto>> GetReviewAsync(long userId, long sessionId);
}
