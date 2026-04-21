using FluentValidation;
using HeThongThiBangLai.Api.DTOs.Exams;

namespace HeThongThiBangLai.Api.Validators.Exams;

public class CreateSampleExamRequestValidator : AbstractValidator<CreateSampleExamRequestDto>
{
    public CreateSampleExamRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(30).WithMessage("Code cannot exceed 30 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name cannot exceed 150 characters");

        RuleFor(x => x.ExamPeriodId)
            .GreaterThan(0).WithMessage("ExamPeriodId must be greater than 0");

        RuleFor(x => x.TotalQuestions)
            .GreaterThan(0).WithMessage("TotalQuestions must be greater than 0");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("DurationMinutes must be greater than 0");
    }
}
