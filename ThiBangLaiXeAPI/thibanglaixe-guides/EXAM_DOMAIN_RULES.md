# Exam Domain Rules

## 1. Goal

This document defines the core business rules for the motorbike theory practice exam system.

It ensures that development, testing, and documentation all follow the same understanding of:
- exam structure,
- critical-question behavior,
- scoring,
- random exam generation,
- wrong-question tracking,
- exam-session lifecycle.

## 2. Main business entities

### 2.1. Question
A theory question in the question bank.

Expected fields may include:
- question content,
- answer options,
- correct answer,
- topic/category,
- difficulty,
- critical-question flag,
- active status.

### 2.2. ExamTemplate
A fixed exam structure or predefined exam set that can be reused.

### 2.3. ExamGenerationRule
Configuration used to generate a random exam.

Typical fields may include:
- total number of questions,
- time limit,
- topic distribution,
- critical-question handling,
- pass threshold.

### 2.4. ExamSession
A candidate's actual exam attempt.

### 2.5. ExamAnswer
A submitted answer item belonging to an exam session.

### 2.6. WrongQuestionRecord
A record of questions the candidate answered incorrectly and may need to review later.

## 3. Exam structure rules

## 3.1. Mandatory principle
The exam structure must be controlled by configuration or template rules whenever possible, not by scattered hard-coded values.

The system should be able to define:
- total number of questions,
- exam duration,
- pass threshold,
- topic distribution,
- whether critical questions are enabled,
- whether auto-submit is enabled at timeout.

## 3.2. Example default reference configuration
A practical reference for v1 may be:
- total questions: `25`
- time limit: `19 minutes`
- pass threshold: `21 correct answers`
- at least one critical question may appear depending on source data
- answering any critical question incorrectly causes failure

> These values should be configurable if the project wants to stay maintainable and future-proof.

## 4. Critical-question rules

### 4.1. Definition
A critical question is a question considered severe enough that answering it incorrectly causes the candidate to fail the exam, regardless of total score.

### 4.2. v1 rule
If the candidate answers **at least one critical question incorrectly**, the exam result must be marked as failed.

### 4.3. Result explanation must be explicit
If failure is caused by a critical question, the result should say so clearly instead of only showing the total score.

## 5. Fixed exam template rules

### 5.1. Fixed exam template
A fixed template may represent:
- a pre-built exam used repeatedly,
- a stable practice set for demonstrations,
- a controlled exercise created by admins/editors.

### 5.2. Template validation rules
A fixed template is valid only if:
- every referenced question exists,
- no duplicate question is included unless explicitly allowed,
- the total number of questions matches the template definition,
- answer and scoring logic can be applied consistently.

## 6. Random exam generation rules

### 6.1. Input
Random exam generation may use:
- a saved exam-generation rule,
- an inline request payload,
- optional filters such as topic or difficulty.

### 6.2. Mandatory rules
The generator must:
- ensure enough questions exist for each required category,
- avoid invalid duplicates unless duplicates are intentionally allowed,
- preserve the configured total number of questions,
- preserve required distribution,
- correctly include or exclude critical questions according to rules.

### 6.3. Rule when data is insufficient
If the question bank is not sufficient to generate a valid exam, the system must:
- reject generation,
- return a clear business error,
- avoid silently generating a broken exam.

## 7. Rules for starting an exam session

Before starting an exam session, the system should:
- verify the candidate is authenticated when required,
- verify the exam template or generation rule exists,
- generate and store the session snapshot if the design needs reproducibility,
- store start time,
- assign duration.

## 8. Rules during the exam

### 8.1. Candidate may
- choose answers,
- change answers before submission if the UX supports it,
- move between questions,
- submit before time runs out.

### 8.2. The system should support
- answer-saving strategy,
- remaining-time tracking,
- safe submission,
- timeout handling.

## 9. Submission rules

When an exam is submitted, the system should:
- stop answer editing,
- collect final answers,
- apply scoring rules,
- check critical-question failure,
- determine pass/fail,
- save result summary,
- record wrong questions for review.

## 10. Scoring formula

### 10.1. Default reference configuration
Suggested default logic:
- score is based on number of correct answers,
- pass if `correctAnswers >= 21` out of `25`,
- fail immediately if any critical question is answered incorrectly.

### 10.2. Displayed score
The result screen should ideally show:
- total correct answers,
- total wrong answers,
- pass/fail status,
- critical-question failure reason if applicable,
- optional list of wrong questions for later review.

### 10.3. Result statuses
Recommended statuses:
- `Passed`
- `Failed`
- `FailedDueToCriticalQuestion`
- `Expired` or `AutoSubmitted` if those states matter in the implementation

## 11. Wrong-question history rules

### 11.1. After each exam
All incorrectly answered questions should be stored or derivable for later review.

### 11.2. Wrong-question review mode
The system should support a mode where candidates can review only the questions they previously answered incorrectly.

### 11.3. Recovery status
If the product later supports progress tracking, a wrong question can be marked as improved or reviewed after repeated correct practice.

## 12. Critical-question practice mode

The system should ideally support a dedicated practice mode focused on critical questions only.
This helps learners pay attention to high-risk knowledge areas.

## 13. Traffic sign rules

Traffic signs are part of the learning domain and should support:
- category,
- name/title,
- image,
- explanation,
- active status.

If traffic sign learning is public, clearly separate public-read endpoints from admin CRUD endpoints.

## 14. Logging and audit rules for exam operations

Important events worth logging:
- exam started,
- exam submitted,
- exam auto-submitted,
- scoring failure,
- invalid generation request,
- admin changes to question bank or exam rules.

## 15. Priority rule when documents conflict

If documents conflict, use this order of priority unless the team defines a newer official source:
1. latest approved project rule/configuration,
2. current system behavior that has been intentionally accepted,
3. progress reports and project documents,
4. older draft documents.

Whenever a conflict is resolved, update the documentation pack accordingly.

## 16. Critical mistakes to avoid

- Hard-coding exam rules in many different places.
- Allowing exam generation with insufficient question data.
- Returning pass even though a critical question was answered incorrectly.
- Losing wrong-question history after submission.
- Mixing content-management logic and scoring logic inside controllers.
- Returning unclear exam results that do not explain why the candidate failed.
