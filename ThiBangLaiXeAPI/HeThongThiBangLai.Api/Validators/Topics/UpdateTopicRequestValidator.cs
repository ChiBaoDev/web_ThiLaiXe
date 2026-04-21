using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Topics;

namespace HeThongThiBangLai.Api.Validators.Topics;

public class UpdateTopicRequestValidator : AbstractValidator<UpdateTopicRequestDto>
{
    public UpdateTopicRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(30).WithMessage("Code cannot exceed 30 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name cannot exceed 150 characters");
    }
}
