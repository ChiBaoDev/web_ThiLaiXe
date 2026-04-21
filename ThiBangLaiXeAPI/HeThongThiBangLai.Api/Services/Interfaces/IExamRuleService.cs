using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.ExamRules;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface IExamRuleService
{
    Task<ApiResponse<List<ExamStructureRuleDto>>> GetListAsync();
    Task<ApiResponse<ExamStructureRuleDto>> GetByIdAsync(long id);
    Task<ApiResponse<ExamStructureRuleDto>> CreateAsync(CreateExamStructureRuleRequestDto request);
    Task<ApiResponse<ExamStructureRuleDto>> UpdateAsync(long id, UpdateExamStructureRuleRequestDto request);
    Task<ApiResponse<ExamStructureRuleDto>> ActivateAsync(long id);
    Task<ApiResponse<ExamRuleValidationResultDto>> ValidateAsync(long id);
    Task DeleteAsync(long id);
}
