using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Files;

namespace HeThongThiBangLai.Api.Validators.Files;

public class CreateFileUsageRequestValidator : AbstractValidator<CreateFileUsageRequestDto>
{
    public CreateFileUsageRequestValidator()
    {
        RuleFor(x => x.EntityName)
            .NotEmpty().WithMessage("EntityName is required")
            .MaximumLength(50).WithMessage("EntityName cannot exceed 50 characters");

        RuleFor(x => x.EntityId)
            .GreaterThan(0).WithMessage("EntityId must be greater than 0");

        RuleFor(x => x.FieldName)
            .NotEmpty().WithMessage("FieldName is required")
            .MaximumLength(50).WithMessage("FieldName cannot exceed 50 characters");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("SortOrder must be greater than or equal to 0");
    }
}
