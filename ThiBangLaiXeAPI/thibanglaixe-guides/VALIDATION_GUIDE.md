# Validation Guide

## 1. Goal

Validation should help the system:
- reject bad input as early as possible,
- reduce database-level bugs,
- return clear errors to frontend and testers,
- keep format validation separate from business logic.

## 2. Recommended tools

### 2.1. Preferred option
Use **FluentValidation** for ASP.NET Core.

Reasons:
- clear rule definitions,
- easy file separation,
- easier unit testing,
- more scalable than relying only on simple attributes.

### 2.2. Acceptable for very small cases
- DataAnnotations for simple modules
- but the project should still aim for one consistent validation style overall

## 3. Recommended validator structure

```text
Validators/
├── Auth/
│   ├── LoginRequestValidator.cs
│   └── RegisterRequestValidator.cs
├── Questions/
│   ├── CreateQuestionRequestValidator.cs
│   └── UpdateQuestionRequestValidator.cs
├── Exams/
│   ├── GenerateRandomExamRequestValidator.cs
│   ├── SubmitExamRequestValidator.cs
│   └── CreateExamTemplateRequestValidator.cs
└── Users/
    └── UpdateUserProfileRequestValidator.cs
```

## 4. Validation categories

### 4.1. Request validation
Checks:
- required fields,
- length,
- format,
- data type,
- range,
- enum value.

### 4.2. Business validation
Checks:
- whether the question bank is large enough to generate an exam,
- whether an exam template matches the declared structure,
- whether the user is allowed to modify the resource,
- whether the exam session has expired.

> Business validation should **not** be pushed entirely into request validators. It belongs primarily in the service or domain-rule layer.

## 5. Validator example

```csharp
public class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Question content is required")
            .MaximumLength(2000).WithMessage("Question content must not exceed 2000 characters");

        RuleFor(x => x.TopicId)
            .GreaterThan(0).WithMessage("Topic is invalid");

        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("Question difficulty is invalid");

        RuleFor(x => x.Answers)
            .NotNull().WithMessage("Answer list is required")
            .Must(x => x.Count >= 2).WithMessage("At least 2 answers are required");
    }
}
```

## 6. Validation rules by module

### 6.1. Auth
- email or username must not be empty,
- password must meet minimum strength,
- confirm password must match.

### 6.2. Question Bank
- content is required,
- topic must be valid,
- difficulty must be valid,
- at least one correct answer is required,
- contradictory states should not be allowed.

### 6.3. Exam Template
- total question count must be greater than zero,
- time limit must be greater than zero,
- distribution values must not be negative,
- total distribution must match total question count.

### 6.4. Random Exam
- exam config ID must be valid, or inline config payload must be valid,
- selected topic must exist if topic-specific mode is used,
- requested quantity must not be negative.

### 6.5. Submit Exam
- exam session ID is required,
- submitted answers must be valid,
- duplicate question IDs in a submit payload must not be allowed.

## 7. Standard validation error response

Example:
```json
{
  "success": false,
  "message": "Invalid input data",
  "data": null,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "field": "content",
      "detail": "Question content is required"
    }
  ],
  "meta": null,
  "timestamp": "2026-04-04T10:00:00Z",
  "traceId": "req-004"
}
```

## 8. Rules for query parameters

Standardize query parameters for list APIs where relevant:
- `page`
- `pageSize`
- `search`
- `sortBy`
- `sortOrder`
- `status`
- `topicId`
- `difficulty`

Example query validator:
```csharp
public class QuestionListQueryValidator : AbstractValidator<QuestionListQuery>
{
    public QuestionListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortOrder).Must(x => x == null || x == "asc" || x == "desc");
    }
}
```

## 9. What not to do

- Do not split validation randomly between controller and service without a rule.
- Do not return raw technical messages directly to clients.
- Do not skip backend validation just because the frontend already validates.

## 10. Validation testing

Each validator should ideally have tests for:
- valid input,
- missing required fields,
- invalid enum values,
- boundary values,
- malformed collections.

## 11. Conclusion

Good validation significantly reduces bugs in exam-related modules, especially around exam templates, random exam generation, and exam submission.
