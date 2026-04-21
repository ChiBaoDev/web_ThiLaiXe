# Prompt Context Guide

## 1. Goal

This document helps team members write better prompts for AI tools when working on the project.

A good prompt should give the AI enough context to:
- understand the project structure,
- follow the architecture,
- respect business rules,
- generate code that fits the repository,
- avoid generic or framework-incompatible output.

## 2. Recommended base prompt

Use a base prompt similar to this when starting a session:

```text
You are helping with the Motorbike Driving License Exam Software project.
The backend uses ASP.NET Core Web API, .NET 8, EF Core, SQL Server, JWT authentication, layered architecture, DTOs, services, repositories, validators, and standardized ApiResponse<T> responses.
The project includes question bank management, exam templates, random exam generation, exam sessions, scoring, critical-question logic, wrong-question review, traffic signs, and admin modules.
When generating code, keep controllers thin, put business logic in services/domain rules, use dedicated DTOs, keep naming in English, and follow the project's response and validation standards.
```

## 3. When asking AI to write code

Always provide:
- the module name,
- the target stack,
- the architectural expectation,
- the exact task,
- relevant request/response examples,
- any important business rule.

### Example of a good prompt

```text
Create a new ASP.NET Core Web API endpoint for submitting an exam session.
Use DTOs, FluentValidation, service layer, repository layer, and ApiResponse<T>.
The endpoint must apply the project's exam rules: fail immediately if any critical question is answered incorrectly, otherwise pass if correct answers are at least 21 out of 25.
Do not put business logic in the controller.
Return a clean C# implementation with request DTO, response DTO, validator, service interface, service implementation, and controller action.
```

## 4. When asking AI to review code

Include:
- the file path,
- the goal of the code,
- what type of review you want.

### Example of a good prompt

```text
Review this AuthController in an ASP.NET Core Web API project.
Check whether it follows layered architecture, response standardization, naming conventions, security basics, and whether business logic is incorrectly placed in the controller.
Then propose a refactor plan.
```

## 5. When asking AI to generate documentation

State clearly:
- the audience,
- the scope,
- the project stack,
- the preferred document style,
- whether the document is intended for developers, testers, supervisors, or AI tools.

## 6. What AI must know about this project

Before generating or reviewing code, AI should know that:
- this is a **motorbike theory practice exam system**,
- the backend is **ASP.NET Core Web API / .NET 8 / EF Core / SQL Server**,
- authentication is **JWT-based**,
- response format should be standardized,
- validation should be separated,
- business rules such as **critical-question failure** are essential,
- exam structure should be configurable,
- wrong-question history matters,
- admin content management is part of the scope.

## 7. Common prompt mistakes

- Giving no project context.
- Asking for code without mentioning the framework.
- Asking AI to write a controller only, without DTO/service/validation context.
- Forgetting to mention business rules like critical-question failure.
- Asking for code that assumes a different stack, such as Node.js or Laravel.
- Forgetting to request standardized API responses.

## 8. Task-specific prompt templates

### 8.1. Creating a new API

```text
Create a new API for [business action] in the Motorbike Driving License Exam Software backend.
Tech stack: ASP.NET Core Web API, .NET 8, EF Core, SQL Server, JWT.
Architecture: Controllers -> Validators -> Services -> Repositories -> DbContext.
Response format: ApiResponse<T>.
Please include DTOs, validator, service interface, service implementation, repository contract if needed, and controller action.
Business rules: [insert rules].
```

### 8.2. Fixing a bug

```text
Analyze and fix this bug in the Motorbike Driving License Exam Software backend.
Please first identify the root cause, then propose the minimal correct fix, and explain whether the issue belongs to validation, business logic, mapping, repository behavior, or API response handling.
Code/context: [paste files or snippets].
```

### 8.3. Architecture review

```text
Review this module in the Motorbike Driving License Exam Software project.
Check folder placement, DTO usage, validation structure, service/repository separation, exception handling, API response consistency, and whether exam-domain rules are implemented in the correct layer.
Then provide a prioritized improvement list.
```

## 9. Short prompt prefix to paste at the start of a session

```text
Project context: Motorbike Driving License Exam Software. Stack: ASP.NET Core Web API, .NET 8, EF Core, SQL Server, JWT. Architecture: Controllers, DTOs, Validators, Services, Repositories, Domain Rules, Common ApiResponse<T>. Key rules: configurable exam structure, critical-question fail logic, wrong-question review, standardized API responses, English naming, thin controllers.
```
