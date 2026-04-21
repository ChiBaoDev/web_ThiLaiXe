using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Auth;

namespace HeThongThiBangLai.Api.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.ten_dang_nhap_hoac_email)
            .NotEmpty().WithMessage("Tên đăng nhập hoặc email không được để trống");

        RuleFor(x => x.mat_khau)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự");
    }
}
