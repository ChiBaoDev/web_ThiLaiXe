# Implementation Roadmap

## 1. Goal

This roadmap provides a practical sequence for continuing the project in a more structured and enterprise-like way without making it too heavy for a course project.

## 2. Short current-state assessment

### 2.1. Strengths
- The backend repository already follows a partially layered direction.
- Authentication and project scaffolding appear to exist.
- The project topic is clear and has a concrete business domain.
- The team already has progress-report documentation that can guide the next phase.

### 2.2. Missing or incomplete areas
- response standardization may still be inconsistent,
- validation structure may still be incomplete,
- domain rules may not yet be centralized,
- some modules may exist only at report level, not at implementation level,
- logging, audit, and documentation discipline may still be weak.

## 3. Priority roadmap

## Phase 1: Standardize the foundation

### Goal
Create a stable backend base that all later modules can follow.

### Tasks
- Standardize `ApiResponse<T>`.
- Add global exception middleware.
- Standardize auth response and auth flow.
- Add validation structure.
- Add common exception types.
- Add common folder structure for responses, helpers, and middleware.
- Clean secrets/config handling if sensitive values are still in tracked files.

### Expected result
A stable foundation where every new module can follow the same architectural pattern.

## Phase 2: Complete core business modules

### Goal
Implement the features that define the product itself.

### Priority order
1. Question bank module
2. Exam template module
3. Random exam generation module
4. Exam session and submission module
5. Scoring and critical-question logic
6. Wrong-question review module
7. Traffic sign module

### Expected result
The system becomes a functioning practice-exam product rather than only an auth-enabled project shell.

## Phase 3: Dashboard and operations

### Tasks
- Add admin dashboard endpoints.
- Add statistics and basic reports.
- Add content-management monitoring.
- Add basic audit logs for important operations.

## Phase 4: UX improvements and expansion

### Optional if time remains
- improve frontend exam workflow,
- add review screens,
- add better filtering and search,
- add progress visualization,
- add refresh token support,
- prepare for mobile or responsive improvements.

## 4. Suggested backend refactor order

A practical order for refactoring the backend:
1. auth response and exception handling,
2. common response wrapper,
3. validators,
4. question module,
5. exam-generation and scoring domain rules,
6. session persistence and history,
7. admin modules and dashboard.

## 5. Completion criteria by phase

### Phase 1 is complete when
- major APIs use standardized responses,
- global exception handling is in place,
- validation structure exists,
- auth flow is cleaner and more predictable,
- architectural rules are documented.

### Phase 2 is complete when
- candidates can take a valid practice exam end to end,
- random generation follows rules,
- scoring works correctly,
- critical-question failure is correct,
- wrong-question review data is stored and retrievable.

### Phase 3 is complete when
- admins can monitor users/content at a basic level,
- key operations are auditable,
- basic statistics are available.

## 6. Biggest risks if the roadmap is ignored

- API style becomes inconsistent across modules.
- Controllers become overloaded with business logic.
- Exam-scoring rules become duplicated and contradictory.
- Future contributors cannot understand where to place new code.
- The team spends time rewriting instead of extending.

## 7. Final recommendation

Do not try to finish everything at once.
First stabilize the architecture and standards, then implement the core business flow, and only after that expand dashboards and quality-of-life features.
