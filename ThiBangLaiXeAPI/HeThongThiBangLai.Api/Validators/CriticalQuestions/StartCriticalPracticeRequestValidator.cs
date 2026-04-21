using FluentValidation;
using HeThongThiBangLai.Api.DTOs.CriticalQuestions;

namespace HeThongThiBangLai.Api.Validators.CriticalQuestions;

public class StartCriticalPracticeRequestValidator : AbstractValidator<StartCriticalPracticeRequestDto>
{
    public StartCriticalPracticeRequestValidator()
    {
        RuleFor(x => x.Size)
            .Must(size => size == 10 || size == 20)
            .WithMessage("Size must be 10 or 20");
    }
}
