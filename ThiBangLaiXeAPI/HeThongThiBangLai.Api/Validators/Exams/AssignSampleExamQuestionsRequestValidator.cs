using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Exams;

namespace HeThongThiBangLai.Api.Validators.Exams;

public class AssignSampleExamQuestionsRequestValidator : AbstractValidator<AssignSampleExamQuestionsRequestDto>
{
    public AssignSampleExamQuestionsRequestValidator()
    {
        RuleFor(x => x.QuestionIds)
            .NotNull().WithMessage("QuestionIds is required")
            .Must(ids => ids.Count > 0).WithMessage("QuestionIds must contain at least 1 item");

        RuleForEach(x => x.QuestionIds)
            .GreaterThan(0).WithMessage("Each question id must be greater than 0");
    }
}
