using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Questions;

namespace HeThongThiBangLai.Api.Validators.Questions;

public class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequestDto>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(x => x.TopicId)
            .GreaterThan(0).WithMessage("TopicId must be greater than 0");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MinimumLength(10).WithMessage("Content must be at least 10 characters")
            .MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters");

        RuleFor(x => x.QuestionType)
            .NotEmpty().WithMessage("QuestionType is required")
            .Must(type => new[] { "MULTIPLE_CHOICE", "TRUE_FALSE" }.Contains(type))
            .WithMessage("QuestionType must be MULTIPLE_CHOICE or TRUE_FALSE");
    }
}
