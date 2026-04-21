using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Auth;

namespace HeThongThiBangLai.Api.Validators.Auth;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ");

        RuleFor(x => x.reset_token)
            .NotEmpty().WithMessage("Token không được để trống");

        RuleFor(x => x.mat_khau_moi)
            .NotEmpty().WithMessage("Mật khẩu mới không được để trống")
            .MinimumLength(8).WithMessage("Mật khẩu mới phải có ít nhất 8 ký tự");
    }
}
