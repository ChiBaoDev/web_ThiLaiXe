using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Entitlements;

namespace HeThongThiBangLai.Api.Validators.Entitlements;

public class UpdateEntitlementPackageRequestValidator : AbstractValidator<UpdateEntitlementPackageRequestDto>
{
    public UpdateEntitlementPackageRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50).WithMessage("Code cannot exceed 50 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name cannot exceed 150 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description != null)
            .WithMessage("Description cannot exceed 500 characters");
    }
}
