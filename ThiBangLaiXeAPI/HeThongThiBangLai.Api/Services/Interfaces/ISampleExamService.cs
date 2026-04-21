using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Exams;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface ISampleExamService
{
    Task<ApiResponse<PagedList<SampleExamDto>>> GetListAsync(int page = 1, int pageSize = 20, string? search = null);
    Task<ApiResponse<SampleExamDto>> GetByIdAsync(long id);
    Task<ApiResponse<SampleExamDto>> CreateAsync(CreateSampleExamRequestDto request);
    Task<ApiResponse<SampleExamDto>> UpdateAsync(long id, UpdateSampleExamRequestDto request);
    Task<ApiResponse<SampleExamDto>> AssignQuestionsAsync(long id, AssignSampleExamQuestionsRequestDto request);
    Task DeleteQuestionAsync(long id, long questionId);
    Task<ApiResponse<SampleExamDto>> PublishAsync(long id);
    Task DeleteAsync(long id);
}
