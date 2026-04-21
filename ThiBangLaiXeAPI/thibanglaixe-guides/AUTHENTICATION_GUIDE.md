# Authentication Guide

## 1. Goal

Authentication and authorization in this project must be strong enough for practice-exam workflows and content management, while still being realistic for a student project.

## 2. Recommended authentication standard

### 2.1. Main approach
- **JWT Bearer Token** for the Web API
- Password hashing using **ASP.NET Core Identity PasswordHasher** or **BCrypt**
- Role-based authorization

### 2.2. Not recommended for the current Web API
- Using server-side session as the primary API authentication mechanism
- Mixing Cookie Auth and JWT unless there is a very specific reason

> If the project later adds a separate MVC or Razor-based server-rendered interface, API authentication and server-rendered authentication should be clearly separated.

## 3. Standard roles

### 3.1. Candidate
Capabilities:
- register and log in,
- view and update own profile,
- take practice exams,
- view own history,
- access learning content.

### 3.2. ContentEditor
Capabilities:
- create and update questions,
- manage traffic sign content,
- manage exam templates and content rules,
- maintain educational materials.

### 3.3. Admin
Capabilities:
- all ContentEditor permissions,
- user and role management,
- system-level configuration,
- dashboard and audit access,
- sensitive administrative operations.

## 4. Recommended token claims

A JWT token should ideally include:
- `sub` or user identifier,
- `email` or username,
- `role`,
- `displayName` if useful,
- `jti` for token tracking,
- expiration metadata.

Example payload idea:
```json
{
  "sub": "user-123",
  "email": "candidate@example.com",
  "role": "Candidate",
  "displayName": "Nguyen Van A",
  "jti": "token-001"
}
```

## 5. Standard auth endpoints

### 5.1. Public
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/forgot-password` (optional)
- `POST /api/auth/reset-password` (optional)

### 5.2. Authenticated
- `GET /api/auth/me`
- `PUT /api/users/profile`
- `POST /api/auth/logout` if token tracking is implemented

### 5.3. Admin only
- `GET /api/admin/users`
- `PUT /api/admin/users/{id}/role`
- `POST /api/admin/users/{id}/lock`

## 6. Sample login response

```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "<jwt>",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "user": {
      "id": "user-123",
      "email": "candidate@example.com",
      "role": "Candidate",
      "displayName": "Nguyen Van A"
    }
  },
  "errors": null,
  "meta": null,
  "timestamp": "2026-04-04T10:00:00Z",
  "traceId": "auth-001"
}
```

## 7. Recommended authorization policies

Suggested policies:
- `CandidateOnly`
- `EditorOrAdmin`
- `AdminOnly`
- `OwnerOrAdmin` for profile/history access if needed

## 8. Security rules

### 8.1. Passwords
- Never store plain text passwords.
- Apply a proper hashing mechanism.
- Enforce a minimum password policy.
- Do not expose password rules unclearly; be explicit in validation messages.

### 8.2. Tokens
- Store JWT secret securely.
- Do not hard-code secrets in source code.
- Keep expiration explicit.
- Validate issuer/audience if the project configuration supports it.

### 8.3. Brute-force mitigation
At minimum, consider:
- failed login logging,
- temporary lock or cooldown after repeated failures,
- throttling or rate limiting for auth endpoints.

### 8.4. Audit
Log important security actions:
- login,
- failed login,
- password reset request,
- role changes,
- account lock/unlock actions.

## 9. Refresh token

### 9.1. For a course-project version
Refresh token support is optional. If the team wants to stay simple, a short-lived access token may be enough.

### 9.2. For a more enterprise-like direction
If the team wants stronger continuity, add:
- refresh token entity/table,
- refresh token rotation,
- revocation support,
- logout invalidation.

## 10. Data access rules

- Candidates can only access their own history and profile.
- Content editors must not access admin-only operations unless explicitly allowed.
- Admins can access system-wide content and management functions.
- Sensitive data should never be returned just because a user is authenticated.

## 11. Forgot password

Recommended only if the team has enough time to implement it safely.
If added, it should include:
- secure reset token generation,
- expiration,
- one-time usage,
- audit logging.

If email integration is too heavy for the course scope, document this feature as a future enhancement instead of building an unsafe shortcut.

## 12. Auth controller coding standard

Auth controllers should:
- accept DTO-based requests,
- rely on validation,
- call a dedicated auth service,
- return standardized responses,
- never embed complex business logic directly in controller actions.

## 13. Conclusion

A simple but disciplined JWT-based authentication model is the most practical choice for this project.
