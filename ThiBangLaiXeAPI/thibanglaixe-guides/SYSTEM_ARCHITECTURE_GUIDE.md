# System Architecture Guide

## 1. Architecture goals

The project architecture should satisfy four practical criteria:
- easy to understand for students and new contributors,
- sufficiently layered for future expansion,
- natural for the .NET + SQL Server ecosystem,
- clear enough that AI tools can generate code within the correct context.

## 2. Recommended architectural direction

### 2.1. Applied architecture
Use a **Layered Architecture** with an enterprise-inspired structure, but keep it lightweight enough for a course project.

Main layers:
1. **Controllers** – HTTP/API layer
2. **Contracts/DTOs** – request and response models
3. **Validators** – request validation layer
4. **Services** – business logic and orchestration layer
5. **Repositories** – data access layer
6. **Data/DbContext** – EF Core and database layer
7. **Domain Rules** – exam generation, scoring, and business constraints
8. **Common** – response wrappers, exceptions, constants, helpers, middleware

## 3. Recommended folder structure

```text
HeThongThiBangLai.Api/
├── Controllers/
│   ├── Auth/
│   ├── QuestionBank/
│   ├── ExamTemplates/
│   ├── RandomExams/
│   ├── ExamSessions/
│   ├── TrafficSigns/
│   ├── Regulations/
│   ├── Users/
│   └── Admin/
│
├── DTOs/
│   ├── Auth/
│   ├── Questions/
│   ├── Exams/
│   ├── TrafficSigns/
│   ├── Regulations/
│   ├── Users/
│   └── Common/
│
├── Validators/
│   ├── Auth/
│   ├── Questions/
│   ├── Exams/
│   ├── TrafficSigns/
│   ├── Regulations/
│   └── Users/
│
├── Services/
│   ├── Interfaces/
│   ├── Auth/
│   ├── Questions/
│   ├── Exams/
│   ├── TrafficSigns/
│   ├── Regulations/
│   └── Users/
│
├── Repositories/
│   ├── Interfaces/
│   ├── Auth/
│   ├── Questions/
│   ├── Exams/
│   ├── TrafficSigns/
│   ├── Regulations/
│   └── Users/
│
├── Domain/
│   ├── Rules/
│   ├── Constants/
│   ├── Enums/
│   └── Specifications/
│
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Configurations/
│   ├── Migrations/
│   └── Seeds/
│
├── Models/
│   └── ... EF entities ...
│
├── Common/
│   ├── Responses/
│   ├── Exceptions/
│   ├── Middleware/
│   ├── Extensions/
│   └── Helpers/
│
├── Mapping/
│   └── AutoMapperProfiles/
│
├── Configurations/
│   ├── JwtOptions.cs
│   ├── SwaggerOptions.cs
│   └── ExamOptions.cs
│
├── Program.cs
└── appsettings.json
```

## 4. Layer dependency model

```text
Controller
  -> Validator
  -> Service
  -> Repository
  -> DbContext
  -> Database
```

Principles:
- Controllers must not access `DbContext` directly.
- Controllers must not score exams or generate questions directly.
- Repositories should not contain complex business rules.
- Services coordinate business flow.

## 5. Module-level architectural standards

### 5.1. Question Bank
- Controller: CRUD APIs for questions
- Service: question-related business checks
- Repository: question queries and filters
- Validator: create/update/import validation rules

### 5.2. Random Exam
- Controller: receives exam-generation requests
- Service: selects questions by structure and rules
- Repository: fetches questions by topic, level, and critical-question flag
- Domain Rules: ensures enough data exists to generate a valid exam

### 5.3. Exam Session
- Controller: starts sessions, saves answers, submits exams
- Service: tracks timing, scoring, and pass/fail logic
- Repository: persists sessions and exam answers
- Domain Rules: critical-question logic, pass thresholds, auto-submit rules

## 6. Standards for entities and DTOs

### 6.1. Entity
- Follow the actual database schema.
- Database naming can remain snake_case if scaffolded or already established.
- Do not expose raw entities directly through the API unless strictly necessary.

### 6.2. DTO
- Use dedicated request/response DTOs.
- Keep DTO naming fully in English and consistent.
- Do not use EF entities as frontend-facing contracts.

Examples:
- `CreateQuestionRequest`
- `UpdateQuestionRequest`
- `QuestionDetailResponse`
- `SubmitExamRequest`
- `ExamResultResponse`

## 7. Required middleware

1. Global exception handling middleware
2. Authentication middleware
3. Authorization middleware
4. Request logging middleware
5. Optional: correlation ID middleware

## 8. Swagger / OpenAPI requirements

Each endpoint should include:
- a clear summary,
- request schema,
- response schema,
- auth requirement,
- example input/output,
- proper status codes.

## 9. Logging

At minimum, log these actions:
- login and logout,
- create/update/delete question,
- create/update exam configuration,
- start and submit exam,
- exam-generation failures,
- scoring failures,
- system errors.

## 10. Current-state vs target architecture

### 10.1. Current state
The repository already has core folders such as `Controllers`, `DTOs`, `Data`, `Models`, `Repositories`, and `Services`, which is a good foundation.

### 10.2. Target next step
The project should add or standardize:
- `Common/Responses`
- `Common/Exceptions`
- `Validators`
- `Domain/Rules`
- `Configurations`
- explicit mapping between entities and DTOs

## 11. Definition of a structurally correct module

A module can be considered architecturally sound when:
- it has dedicated request/response DTOs,
- it has dedicated validators,
- it has a service interface and implementation,
- it has repository abstraction if data access is non-trivial,
- the controller is thin and orchestration-focused,
- Swagger is clear,
- response format is standardized,
- logging and error handling follow project rules.
