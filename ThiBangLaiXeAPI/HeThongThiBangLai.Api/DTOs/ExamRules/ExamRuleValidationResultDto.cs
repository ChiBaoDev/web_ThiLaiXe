namespace HeThongThiBangLai.Api.DTOs.ExamRules;

public class ExamRuleValidationResultDto
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = [];
}
