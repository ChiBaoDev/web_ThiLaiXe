using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Cms;

namespace HeThongThiBangLai.Api.Validators.Cms;

public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequestDto>
{
    public UpdatePostRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50).WithMessage("Code cannot exceed 50 characters");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title cannot exceed 255 characters");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .MaximumLength(255).WithMessage("Slug cannot exceed 255 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required");

        RuleFor(x => x.PostType)
            .NotEmpty().WithMessage("PostType is required")
            .Must(x => new[] { "gioi_thieu", "tin_tuc", "khoa_hoc", "huong_dan" }.Contains(x))
            .WithMessage("PostType must be one of: gioi_thieu, tin_tuc, khoa_hoc, huong_dan");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(x => new[] { "draft", "published", "archived" }.Contains(x))
            .WithMessage("Status must be one of: draft, published, archived");

        RuleForEach(x => x.CategoryIds)
            .GreaterThan(0).WithMessage("Each CategoryId must be greater than 0");

        RuleFor(x => x.MetaDescription)
            .MaximumLength(500).When(x => x.MetaDescription != null)
            .WithMessage("MetaDescription cannot exceed 500 characters");

        RuleFor(x => x.Summary)
            .MaximumLength(1000).When(x => x.Summary != null)
            .WithMessage("Summary cannot exceed 1000 characters");

        RuleFor(x => x.CanonicalUrl)
            .MaximumLength(500).When(x => x.CanonicalUrl != null)
            .WithMessage("CanonicalUrl cannot exceed 500 characters");
    }
}
