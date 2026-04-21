using FluentValidation;
using HeThongThiBangLai.Api.DTOs.ExamRules;

namespace HeThongThiBangLai.Api.Validators.Exams;

public class ExamRuleTopicAllocationValidator : AbstractValidator<ExamRuleTopicAllocationDto>
{
    public ExamRuleTopicAllocationValidator()
    {
        RuleFor(x => x.TopicId)
            .GreaterThan(0);

        RuleFor(x => x.QuestionCount)
            .GreaterThan(0);
    }
}
