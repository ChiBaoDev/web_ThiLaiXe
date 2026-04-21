# Project Context

## 1. Project name

**Motorbike Driving License Exam Software**

Suggested internal repository naming:
- `ThiBangLaiXe`
- `MotorbikeExamSystem`
- `DrivingLicensePracticeSystem`

## 2. Problem the project solves

This project is intended to support learners preparing for the **motorbike driving license theory exam**.

The system should provide a practical digital platform where users can:
- read exam regulations,
- access and review the question bank,
- take practice exams,
- review wrong answers,
- review critical questions,
- study traffic signs,
- track personal practice history.

At the administration level, the system should allow authorized users to:
- manage question data,
- manage traffic signs,
- manage exam templates,
- define exam generation rules,
- monitor user activity and exam statistics.

## 3. Main goals of the current version

### 3.1. Business goals
- Provide a usable practice-exam workflow for candidates.
- Support fixed exam templates and random exam generation.
- Handle scoring, pass/fail logic, and critical-question rules correctly.
- Store exam history and wrong-question records.
- Provide a manageable admin-side content workflow.

### 3.2. Technical goals
- Standardize backend architecture for long-term maintainability.
- Make API contracts explicit and consistent.
- Separate validation, business logic, and data access.
- Support Swagger/OpenAPI-based testing and documentation.
- Make the codebase easier to continue as a course project in a more enterprise-like manner.

## 4. Standard user roles

### 4.1. Candidate
The end user who practices for the theory exam.

Candidate capabilities:
- register and log in,
- view own profile,
- take practice exams,
- submit answers,
- view own results and history,
- review wrong questions,
- review critical questions,
- study traffic signs and exam regulations.

### 4.2. ContentEditor
A content manager role for learning materials and exam data.

ContentEditor capabilities:
- create and update questions,
- create and update traffic signs,
- manage exam templates,
- manage exam rules and content categories,
- review content quality.

### 4.3. Admin
The highest-access role in the system.

Admin capabilities:
- all ContentEditor permissions,
- user and role management,
- system configuration,
- dashboard and audit review,
- important content approval if approval flow is added.

## 5. Standard business modules

### 5.1. ExamRegulationModule
Stores exam regulations, rule summaries, and configuration references shown to users.

### 5.2. QuestionBankModule
Handles the question bank, answer options, difficulty, topic/category, and critical-question flags.

### 5.3. ExamTemplateModule
Stores fixed exam templates used for stable or predefined practice sessions.

### 5.4. RandomExamModule
Generates random exams according to configurable structure and domain rules.

### 5.5. ExamRunnerModule
Handles starting an exam, saving answers, timing, submission, and result generation.

### 5.6. WrongQuestionPracticeModule
Tracks wrongly answered questions and supports focused review.

### 5.7. CriticalQuestionPracticeModule
Supports special practice mode for critical questions.

### 5.8. TrafficSignModule
Stores traffic sign data, categories, images, and learning content.

### 5.9. UserAndHistoryModule
Handles user profile, practice history, statistics, and learning progress.

### 5.10. AdminDashboardModule
Provides dashboard metrics, content status, and operational summaries for administrators.

## 6. Scope rules

### 6.1. In scope for v1
- Authentication and authorization
- Question bank management
- Traffic sign management
- Fixed exam templates
- Random exam generation based on configurable rules
- Exam session workflow
- Scoring and pass/fail logic
- Wrong-question history
- Critical-question review mode
- Basic admin-side content management

### 6.2. Out of scope for v1 or later-phase work
- Payment
- Real-world training center integration
- Official government exam synchronization
- Advanced analytics or AI recommendations
- Multi-tenant architecture
- Native mobile apps
- Complex approval workflows

## 7. Current repository interpretation

Based on the available repository structure, the backend already contains foundation folders such as:
- `Controllers`
- `DTOs`
- `Data`
- `Models`
- `Repositories`
- `Services`

This is a good starting point for a layered architecture, but the project still needs stronger standardization in:
- unified API responses,
- validation structure,
- domain rules,
- logging and exception handling,
- documentation-driven implementation.

## 8. Most important project principles

### 8.1. Exam structure must be configurable
The number of questions, time limit, topic distribution, and critical-question behavior should not be hard-coded when the system can reasonably support configuration.

### 8.2. Business rules must live in the service/domain layer
Controllers should not perform scoring, random exam generation, or pass/fail logic directly.

### 8.3. API responses must be consistent
All APIs should follow one common response contract.

### 8.4. Correct exam logic matters more than quick delivery
A practice exam system fails its purpose if scoring or critical-question behavior is wrong.

### 8.5. Documentation must evolve together with code
When the team changes structure or rules, the corresponding guides must be updated as part of the work.
