# Coding Convention

## 1. Goal

This convention exists to make the project:
- easier to read,
- easier to review,
- easier to maintain,
- easier for team members and AI tools to extend consistently.

## 2. General principles

- Prefer clarity over cleverness.
- Keep naming explicit.
- Separate concerns by layer.
- Keep controller actions thin.
- Avoid mixed naming styles across modules.
- When in doubt, follow the existing standard instead of inventing a new one.

## 3. Language and naming rules

### 3.1. In C# code
Use **English only** for:
- class names,
- interface names,
- method names,
- variable names,
- DTO names,
- enum names,
- comments that are meant to remain in the codebase.

### 3.2. In the database
If the current scaffolded schema already uses snake_case or another established convention, keep it stable unless the team is intentionally refactoring the database.

### 3.3. In API response messages
Use English consistently if the API is standardized in English.
Avoid mixing languages inside the same API set.

## 4. Naming convention

### 4.1. Class / Interface
- Class: `QuestionService`, `ExamSessionController`
- Interface: `IQuestionRepository`, `IAuthService`
- Validator: `CreateQuestionRequestValidator`

### 4.2. Method / Variable / Parameter
- Method: `GenerateRandomExamAsync`
- Variable: `questionCount`
- Parameter: `candidateId`

### 4.3. Constant
Use `PascalCase` or `UPPER_CASE` based on project preference, but stay consistent.

Examples:
- `DefaultPageSize`
- `MaxQuestionPerExam`

### 4.4. File name
One main class per file.
The file name should match the main class name.

Examples:
- `QuestionService.cs`
- `SubmitExamRequest.cs`
- `AuthController.cs`

## 5. Controller standard

Controllers should:
- receive HTTP requests,
- validate request shape,
- delegate to services,
- return standardized responses,
- avoid direct database work,
- avoid business-heavy code.

## 6. Service standard

Services should:
- contain business logic,
- coordinate repositories,
- apply domain rules,
- throw standardized exceptions when needed,
- avoid returning raw EF entities unless justified.

## 7. Repository standard

Repositories should:
- handle data access,
- contain query logic,
- avoid business decision-making,
- keep database interaction isolated from controllers.

## 8. DTO standard

### 8.1. Request DTO
Request DTOs should describe input from clients only.
Do not overload them with internal-only fields.

Examples:
- `LoginRequest`
- `CreateQuestionRequest`
- `SubmitExamRequest`

### 8.2. Response DTO
Response DTOs should be shaped for API consumers.
Do not expose extra internal data by default.

Examples:
- `UserProfileResponse`
- `QuestionDetailResponse`
- `ExamResultResponse`

## 9. Enum standard

Use enums for stable value sets such as:
- role type,
- exam status,
- question difficulty,
- traffic sign category.

Keep enum naming explicit and readable.

## 10. Validation standard

- Prefer dedicated validators.
- Do not hide validation inside random service methods without structure.
- Keep format validation separate from deeper business validation.

## 11. Error handling

- Use standardized exceptions.
- Do not leak raw technical stack traces to clients.
- Handle application-level exceptions in a global exception middleware.
- Keep controller-level try/catch blocks minimal and justified.

## 12. Logging

Log important business operations and failures.
Avoid noisy logging that makes real issues hard to find.

Good candidates for logging:
- authentication events,
- question management,
- exam submission,
- scoring failures,
- admin changes.

## 13. Database convention

### 13.1. Tables / columns
Keep naming stable and consistent with the current schema.
Avoid partial renaming across only some modules.

### 13.2. Keys
- primary keys should be explicit,
- foreign keys should be named consistently,
- relationship mappings should be readable in EF Core configuration.

### 13.3. Recommended audit fields when appropriate
- `created_at`
- `created_by`
- `updated_at`
- `updated_by`
- `is_deleted` or soft-delete field when relevant

## 14. Git convention

### 14.1. Branch naming
Examples:
- `feature/question-bank-api`
- `fix/auth-login-response`
- `refactor/exam-session-service`

### 14.2. Commit message
Prefer short, specific commit messages.

Examples:
- `feat: add create question API`
- `fix: correct critical question fail logic`
- `refactor: standardize api response wrapper`

## 15. Test naming

Use clear, behavior-focused names.

Examples:
- `CreateQuestion_ShouldReturnValidationError_WhenContentIsEmpty`
- `SubmitExam_ShouldFail_WhenCriticalQuestionIsWrong`

## 16. Pull request review standard

A pull request should be reviewed for:
- architectural correctness,
- naming consistency,
- validation completeness,
- API response consistency,
- business rule correctness,
- test coverage where applicable,
- documentation updates if the change affects structure or rules.
