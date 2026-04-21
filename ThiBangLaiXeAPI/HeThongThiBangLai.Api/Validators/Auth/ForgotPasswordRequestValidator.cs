using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Auth;

namespace HeThongThiBangLai.Api.Validators.Auth;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ");
    }
}
