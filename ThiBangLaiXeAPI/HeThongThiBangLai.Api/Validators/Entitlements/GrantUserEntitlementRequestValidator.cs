using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Entitlements;

namespace HeThongThiBangLai.Api.Validators.Entitlements;

public class GrantUserEntitlementRequestValidator : AbstractValidator<GrantUserEntitlementRequestDto>
{
    public GrantUserEntitlementRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0");

        RuleFor(x => x.PackageId)
            .GreaterThan(0).WithMessage("PackageId must be greater than 0");

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("Source is required")
            .Must(x => new[] { "payment", "manual", "promo" }.Contains(x))
            .WithMessage("Source must be one of: payment, manual, promo");

        RuleFor(x => x.ExpiresAt)
            .Must((x, expiresAt) => !expiresAt.HasValue || expiresAt.Value >= x.EffectiveFrom)
            .WithMessage("ExpiresAt must be greater than or equal to EffectiveFrom");

        RuleFor(x => x.Note)
            .MaximumLength(500).When(x => x.Note != null)
            .WithMessage("Note cannot exceed 500 characters");
    }
}
