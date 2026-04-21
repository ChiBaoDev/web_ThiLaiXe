# API Creation Rules

## 1. Goal

This document defines the standard process for adding a new API or module to the project.

Every new API should be:
- aligned with business context,
- aligned with architecture,
- aligned with the response standard,
- easy to test,
- easy to review,
- safe for the project structure.

## 2. Pre-coding checklist

Before creating a new API, answer these questions:
1. Which module does this API belong to?
2. Is it a simple CRUD action or a business workflow?
3. What permission is required?
4. What validation is required?
5. What response should it return?
6. Does it affect exam/scoring rules?
7. Does it require logging or audit tracking?

## 3. Standard API creation workflow

## Step 1: Identify the module

Examples:
- questions -> `QuestionBank`
- exam templates -> `ExamTemplates`
- random exam generation -> `RandomExams`
- answer submission -> `ExamSessions`

## Step 2: Define the contract

Create request and response DTOs first.

Examples:
- `CreateQuestionRequest`
- `QuestionDetailResponse`

Do not start with the controller before the contract is clear.

## Step 3: Create the validator

Create a dedicated validator for the request.

Examples:
- `CreateQuestionRequestValidator`
- `SubmitExamRequestValidator`

## Step 4: Create or update the service

The service should contain:
- business logic,
- orchestration,
- repository calls,
- standardized exceptions when needed.

## Step 5: Create or update the repository

The repository should only handle data access and query logic.

## Step 6: Create the controller action

The controller receives the request, calls the service, and returns `ApiResponse<T>`.

## Step 7: Update Swagger

Every endpoint must have:
- summary,
- request body,
- response type,
- status codes,
- auth requirement.

## Step 8: Write minimum tests

At minimum, add:
- validator tests or service tests,
- a happy path,
- the main failure path.

## 4. Standard template for a CRUD module

### 4.1. Required files

```text
DTOs/Questions/
├── CreateQuestionRequest.cs
├── UpdateQuestionRequest.cs
└── QuestionDetailResponse.cs

Validators/Questions/
├── CreateQuestionRequestValidator.cs
└── UpdateQuestionRequestValidator.cs

Services/Interfaces/
└── IQuestionService.cs

Services/Questions/
└── QuestionService.cs

Repositories/Interfaces/
└── IQuestionRepository.cs

Repositories/Questions/
└── QuestionRepository.cs

Controllers/QuestionBank/
└── QuestionController.cs
```

## 5. Standard template for a business-flow API

Example: `SubmitExam`

### 5.1. Required files

```text
DTOs/Exams/
├── SubmitExamRequest.cs
└── ExamResultResponse.cs

Validators/Exams/
└── SubmitExamRequestValidator.cs

Services/Interfaces/
└── IExamSessionService.cs

Services/Exams/
└── ExamSessionService.cs

Repositories/Interfaces/
├── IExamSessionRepository.cs
├── IExamAnswerRepository.cs
└── IQuestionRepository.cs

Controllers/ExamSessions/
└── ExamSessionController.cs
```

## 6. Rules by endpoint type

### 6.1. GET list
Support these when relevant:
- paging,
- search,
- sort,
- filter.

### 6.2. GET detail
Return `404` when the resource does not exist.

### 6.3. POST create
- validate request,
- check business rules,
- return `201` when creation succeeds.

### 6.4. PUT/PATCH update
- check that the resource exists,
- check edit permission,
- update only allowed fields.

### 6.5. DELETE
- define whether it is hard delete or soft delete,
- for important data, prefer soft delete when reasonable.

## 7. Authentication and authorization rules

### 7.1. Public endpoints
Only for actions such as:
- login and registration,
- viewing exam regulations,
- optionally viewing traffic signs if the system allows public access.

### 7.2. Candidate endpoints
- practice exams,
- personal history,
- personal profile.

### 7.3. Editor/Admin endpoints
- question CRUD,
- traffic sign CRUD,
- regulation CRUD,
- exam template management,
- exam-generation configuration.

## 8. Response rules

All new APIs must follow `API_RESPONSE_GUIDE.md`.

Not acceptable:
- endpoint A returns `{ message: ... }`
- endpoint B returns a raw object
- endpoint C returns `{ status: true }`

## 9. Exception rules

Services may throw:
- `NotFoundAppException`
- `ValidationAppException`
- `BusinessRuleAppException`
- `ForbiddenAppException`

Controllers should not repeat try/catch in every action if the system already has a global exception middleware.

## 10. Exam business-rule rules

If the API is related to:
- exam generation,
- scoring,
- critical questions,
- wrong-question history,

then `EXAM_DOMAIN_RULES.md` must be reviewed before coding.

## 11. Mapping rules

- Map entity to DTO explicitly.
- Do not expose all entity fields by default.
- Keep response DTOs stable for frontend consumers.
- Use AutoMapper if the team adopts it consistently; otherwise use explicit manual mapping.

## 12. Example: add API for creating an exam template

### 12.1. DTO
- `CreateExamTemplateRequest`
- `ExamTemplateDetailResponse`

### 12.2. Validator
- validate template name,
- validate total question count,
- validate distribution rules,
- validate time limit.

### 12.3. Service
The service should:
- validate business rules,
- ensure distribution totals are correct,
- persist the template,
- return the response DTO.

### 12.4. Controller
The controller should:
- receive request DTO,
- rely on validator + service,
- return `201 Created` with `ApiResponse<ExamTemplateDetailResponse>`.

## 13. Definition of Done for a new API

A new API is considered done only when:
- module placement is correct,
- request/response DTOs are complete,
- validation is implemented,
- business logic is placed in service/domain layer,
- response follows the common standard,
- Swagger is updated,
- the main success and failure paths are covered,
- the API is consistent with project rules and documentation.
