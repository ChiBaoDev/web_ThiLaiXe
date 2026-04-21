using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Auth;

namespace HeThongThiBangLai.Api.Validators.Auth;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.mat_khau_cu)
            .NotEmpty().WithMessage("Mật khẩu cũ không được để trống");

        RuleFor(x => x.mat_khau_moi)
            .NotEmpty().WithMessage("Mật khẩu mới không được để trống")
            .MinimumLength(8).WithMessage("Mật khẩu mới phải có ít nhất 8 ký tự");
    }
}
