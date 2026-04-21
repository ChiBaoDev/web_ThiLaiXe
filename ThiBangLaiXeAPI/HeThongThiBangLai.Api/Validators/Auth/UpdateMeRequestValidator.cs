using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Auth;

namespace HeThongThiBangLai.Api.Validators.Auth;

public class UpdateMeRequestValidator : AbstractValidator<UpdateMeRequestDto>
{
    public UpdateMeRequestValidator()
    {
        RuleFor(x => x.ho_ten)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự");

        RuleFor(x => x.email)
            .EmailAddress().WithMessage("Email không hợp lệ");
    }
}
