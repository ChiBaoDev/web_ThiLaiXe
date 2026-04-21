using FluentValidation;
using HeThongThiBangLai.Api.DTOs.ExamSessions;

namespace HeThongThiBangLai.Api.Validators.Exams;

public class SubmitExamAnswerRequestValidator : AbstractValidator<SubmitExamAnswerRequestDto>
{
    public SubmitExamAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0);

        RuleFor(x => x.AnswerId)
            .GreaterThan(0);
    }
}
