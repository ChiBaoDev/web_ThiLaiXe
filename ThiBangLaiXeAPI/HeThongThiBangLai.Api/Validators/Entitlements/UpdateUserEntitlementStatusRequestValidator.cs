using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Entitlements;

namespace HeThongThiBangLai.Api.Validators.Entitlements;

public class UpdateUserEntitlementStatusRequestValidator : AbstractValidator<UpdateUserEntitlementStatusRequestDto>
{
    public UpdateUserEntitlementStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(x => new[] { "active", "expired", "revoked" }.Contains(x))
            .WithMessage("Status must be one of: active, expired, revoked");

        RuleFor(x => x.Note)
            .MaximumLength(500).When(x => x.Note != null)
            .WithMessage("Note cannot exceed 500 characters");
    }
}
