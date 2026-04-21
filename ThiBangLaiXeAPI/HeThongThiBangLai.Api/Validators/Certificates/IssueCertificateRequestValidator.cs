using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Certificates;

namespace HeThongThiBangLai.Api.Validators.Certificates;

public class IssueCertificateRequestValidator : AbstractValidator<IssueCertificateRequestDto>
{
    public IssueCertificateRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50).WithMessage("Code cannot exceed 50 characters");

        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("StudentId must be greater than 0");

        RuleFor(x => x.ExamResultId)
            .GreaterThan(0).WithMessage("ExamResultId must be greater than 0");

        RuleFor(x => x.IssuedAt)
            .NotEmpty().WithMessage("IssuedAt is required");

        RuleFor(x => x.ExpiresAt)
            .Must((x, expiresAt) => !expiresAt.HasValue || expiresAt.Value >= x.IssuedAt)
            .WithMessage("ExpiresAt must be greater than or equal to IssuedAt");

        RuleFor(x => x.CertificateFileId)
            .GreaterThan(0).When(x => x.CertificateFileId.HasValue)
            .WithMessage("CertificateFileId must be greater than 0");
    }
}
