using FluentValidation;
using HeThongThiBangLai.Api.DTOs.ExamRules;

namespace HeThongThiBangLai.Api.Validators.Exams;

public class ExamRuleDifficultyAllocationValidator : AbstractValidator<ExamRuleDifficultyAllocationDto>
{
    public ExamRuleDifficultyAllocationValidator()
    {
        RuleFor(x => x.Difficulty)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.QuestionCount)
            .GreaterThan(0);
    }
}
