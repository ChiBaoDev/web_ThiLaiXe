using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Certificates;

namespace HeThongThiBangLai.Api.Validators.Certificates;

public class UpdateCertificateStatusRequestValidator : AbstractValidator<UpdateCertificateStatusRequestDto>
{
    public UpdateCertificateStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(x => new[] { "valid", "revoked", "expired" }.Contains(x))
            .WithMessage("Status must be one of: valid, revoked, expired");
    }
}
