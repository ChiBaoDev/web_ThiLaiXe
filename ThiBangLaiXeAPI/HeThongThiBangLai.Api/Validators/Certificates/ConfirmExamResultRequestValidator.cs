using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Certificates;

namespace HeThongThiBangLai.Api.Validators.Certificates;

public class ConfirmExamResultRequestValidator : AbstractValidator<ConfirmExamResultRequestDto>
{
    public ConfirmExamResultRequestValidator()
    {
        RuleFor(x => x.Result)
            .NotEmpty().WithMessage("Result is required")
            .Must(x => new[] { "dat", "khong_dat" }.Contains(x))
            .WithMessage("Result must be one of: dat, khong_dat");
    }
}
