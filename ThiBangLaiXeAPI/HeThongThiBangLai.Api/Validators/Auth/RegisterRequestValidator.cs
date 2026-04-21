using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Auth;

namespace HeThongThiBangLai.Api.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.ten_dang_nhap)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
            .MinimumLength(3).WithMessage("Tên đăng nhập phải có ít nhất 3 ký tự")
            .MaximumLength(50).WithMessage("Tên đăng nhập không được vượt quá 50 ký tự");

        RuleFor(x => x.email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ");

        RuleFor(x => x.mat_khau)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự");

        RuleFor(x => x.ho_ten)
            .NotEmpty().WithMessage("Họ tên không được để trống");
    }
}
