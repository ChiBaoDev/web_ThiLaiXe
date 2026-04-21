using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Files;

namespace HeThongThiBangLai.Api.Validators.Files;

public class CreateFileRequestValidator : AbstractValidator<CreateFileRequestDto>
{
    public CreateFileRequestValidator()
    {
        RuleFor(x => x.StorageProvider)
            .NotEmpty().WithMessage("StorageProvider is required")
            .Must(provider => new[] { "local", "s3", "cloudinary", "azure_blob", "gcs" }.Contains(provider))
            .WithMessage("StorageProvider must be one of: local, s3, cloudinary, azure_blob, gcs");

        RuleFor(x => x.ObjectKey)
            .NotEmpty().WithMessage("ObjectKey is required")
            .MaximumLength(500).WithMessage("ObjectKey cannot exceed 500 characters");

        RuleFor(x => x.PublicUrl)
            .NotEmpty().WithMessage("PublicUrl is required")
            .MaximumLength(1000).WithMessage("PublicUrl cannot exceed 1000 characters");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("FileName is required")
            .MaximumLength(255).WithMessage("FileName cannot exceed 255 characters");

        RuleFor(x => x.MimeType)
            .NotEmpty().WithMessage("MimeType is required")
            .MaximumLength(100).WithMessage("MimeType cannot exceed 100 characters");

        RuleFor(x => x.SizeBytes)
            .GreaterThanOrEqualTo(0).WithMessage("SizeBytes must be greater than or equal to 0");

        RuleFor(x => x.Width)
            .GreaterThanOrEqualTo(0).When(x => x.Width.HasValue)
            .WithMessage("Width must be greater than or equal to 0");

        RuleFor(x => x.Height)
            .GreaterThanOrEqualTo(0).When(x => x.Height.HasValue)
            .WithMessage("Height must be greater than or equal to 0");

        RuleFor(x => x.DurationSeconds)
            .GreaterThanOrEqualTo(0).When(x => x.DurationSeconds.HasValue)
            .WithMessage("DurationSeconds must be greater than or equal to 0");
    }
}
