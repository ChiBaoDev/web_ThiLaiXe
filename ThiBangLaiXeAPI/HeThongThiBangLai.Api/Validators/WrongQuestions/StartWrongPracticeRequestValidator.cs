using FluentValidation;
using HeThongThiBangLai.Api.DTOs.WrongQuestions;

namespace HeThongThiBangLai.Api.Validators.WrongQuestions;

public class StartWrongPracticeRequestValidator : AbstractValidator<StartWrongPracticeRequestDto>
{
    public StartWrongPracticeRequestValidator()
    {
        RuleFor(x => x.Size)
            .NotEmpty().WithMessage("Practice size is required")
            .Must(size => size == 10 || size == 20)
            .WithMessage("Practice size must be 10 or 20");
    }
}
