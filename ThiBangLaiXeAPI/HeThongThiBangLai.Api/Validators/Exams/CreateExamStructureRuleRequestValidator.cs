using FluentValidation;
using HeThongThiBangLai.Api.DTOs.ExamRules;

namespace HeThongThiBangLai.Api.Validators.Exams;

public class CreateExamStructureRuleRequestValidator : AbstractValidator<CreateExamStructureRuleRequestDto>
{
    public CreateExamStructureRuleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.TotalQuestions)
            .GreaterThan(0);

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0);

        RuleFor(x => x.PassingCorrectAnswers)
            .GreaterThan(0);

        RuleFor(x => x.RequiredCriticalQuestions)
            .GreaterThanOrEqualTo(0);

        RuleForEach(x => x.TopicAllocations)
            .SetValidator(new ExamRuleTopicAllocationValidator());

        RuleForEach(x => x.DifficultyAllocations)
            .SetValidator(new ExamRuleDifficultyAllocationValidator());
    }
}
