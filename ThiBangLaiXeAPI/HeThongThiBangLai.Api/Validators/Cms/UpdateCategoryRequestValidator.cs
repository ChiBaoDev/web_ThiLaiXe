using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Cms;

namespace HeThongThiBangLai.Api.Validators.Cms;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequestDto>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.ParentId)
            .GreaterThan(0).When(x => x.ParentId.HasValue)
            .WithMessage("ParentId must be greater than 0");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50).WithMessage("Code cannot exceed 50 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name cannot exceed 150 characters");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .MaximumLength(200).WithMessage("Slug cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description != null)
            .WithMessage("Description cannot exceed 500 characters");
    }
}
