using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Files;

namespace HeThongThiBangLai.Api.Validators.Files;

public class UpdateFileRequestValidator : AbstractValidator<UpdateFileRequestDto>
{
    public UpdateFileRequestValidator()
    {
        RuleFor(x => x.PublicUrl)
            .MaximumLength(1000).When(x => x.PublicUrl != null)
            .WithMessage("PublicUrl cannot exceed 1000 characters");

        RuleFor(x => x.FileName)
            .MaximumLength(255).When(x => x.FileName != null)
            .WithMessage("FileName cannot exceed 255 characters");

        RuleFor(x => x.MimeType)
            .MaximumLength(100).When(x => x.MimeType != null)
            .WithMessage("MimeType cannot exceed 100 characters");

        RuleFor(x => x.SizeBytes)
            .GreaterThanOrEqualTo(0).When(x => x.SizeBytes.HasValue)
            .WithMessage("SizeBytes must be greater than or equal to 0");

        RuleFor(x => x.Width)
            .GreaterThanOrEqualTo(0).When(x => x.Width.HasValue)
            .WithMessage("Width must be greater than or equal to 0");

        RuleFor(x => x.Height)
            .GreaterThanOrEqualTo(0).When(x => x.Height.HasValue)
            .WithMessage("Height must be greater than or equal to 0");

        RuleFor(x => x.DurationSeconds)
            .GreaterThanOrEqualTo(0).When(x => x.DurationSeconds.HasValue)
            .WithMessage("DurationSeconds must be greater than or equal to 0");

        RuleFor(x => x.Status)
            .Must(status => status == null || new[] { "active", "archived", "deleted" }.Contains(status))
            .WithMessage("Status must be one of: active, archived, deleted");
    }
}
