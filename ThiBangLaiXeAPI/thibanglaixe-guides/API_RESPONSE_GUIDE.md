# API Response Guide

## 1. Goal

All APIs in the project must return a **unified response structure** so that:
- frontend code is easier to handle,
- testers can validate behavior consistently,
- logs are easier to read,
- AI can generate code with the correct format,
- API documentation stays consistent across modules.

## 2. Standard response formats

### 2.1. Success response
```json
{
  "success": true,
  "message": "Data retrieved successfully",
  "data": {},
  "errors": null,
  "meta": null,
  "timestamp": "2026-04-04T10:00:00Z",
  "traceId": "abc-123"
}
```

### 2.2. Error response
```json
{
  "success": false,
  "message": "Invalid input data",
  "data": null,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "field": "email",
      "detail": "Email format is invalid"
    }
  ],
  "meta": null,
  "timestamp": "2026-04-04T10:00:00Z",
  "traceId": "abc-123"
}
```

### 2.3. List response
```json
{
  "success": true,
  "message": "List retrieved successfully",
  "data": [
    {
      "id": 1,
      "name": "Question 1"
    }
  ],
  "errors": null,
  "meta": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 100,
    "totalPages": 10
  },
  "timestamp": "2026-04-04T10:00:00Z",
  "traceId": "abc-123"
}
```

## 3. Recommended C# models

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<ApiError>? Errors { get; set; }
    public PaginationMeta? Meta { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
}

public class ApiError
{
    public string Code { get; set; } = string.Empty;
    public string? Field { get; set; }
    public string Detail { get; set; } = string.Empty;
}

public class PaginationMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
```

## 4. Standard HTTP status codes

- `200 OK`: data retrieval or successful update
- `201 Created`: successful creation
- `204 No Content`: successful delete or logout
- `400 Bad Request`: malformed request or validation-format issue
- `401 Unauthorized`: missing or invalid authentication
- `403 Forbidden`: authenticated but not allowed
- `404 Not Found`: resource not found
- `409 Conflict`: data conflict
- `422 Unprocessable Entity`: valid format but business rule violation
- `500 Internal Server Error`: unexpected system error

## 5. Standardized exceptions

A custom base exception is recommended:

```csharp
public class AppException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }
    public string? Field { get; }

    public AppException(string message, string errorCode, int statusCode = 400, string? field = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Field = field;
    }
}
```

Recommended derived exceptions:
- `ValidationAppException`
- `NotFoundAppException`
- `ForbiddenAppException`
- `BusinessRuleAppException`

## 6. Global exception middleware

Do not scatter exception handling across every controller action unless there is a good reason.

Recommended flow:
- service/business layer throws standardized exceptions,
- middleware catches them,
- middleware returns the common `ApiResponse<T>` structure.

Pseudo-flow:
```text
Controller -> Service throws AppException -> GlobalExceptionMiddleware -> ApiResponse<T>
```

## 7. Recommended helper methods

Create a response helper/factory so controllers stay thin:

```csharp
public static class ApiResponseFactory
{
    public static ApiResponse<T> Success<T>(T data, string message = "Success") => ...;
    public static ApiResponse<T> Created<T>(T data, string message = "Created successfully") => ...;
    public static ApiResponse<object> Fail(string message, List<ApiError>? errors = null) => ...;
}
```

## 8. Controller rules

### 8.1. Do not
- Do not return `Ok(entity)` directly for business APIs.
- Do not return a different anonymous object shape in every action.
- Do not mix English and Vietnamese messages randomly.

### 8.2. Must do
- Return `ApiResponse<T>` consistently.
- Use short, clear messages.
- For paged list APIs, include `meta`.
- For validation errors, include detailed `errors` entries.

## 9. Rules for list/filter/search APIs

If an API supports pagination, the response must include:
- `data`: list of items
- `meta.page`
- `meta.pageSize`
- `meta.totalItems`
- `meta.totalPages`

## 10. Project-specific examples

### 10.1. Create question successfully
```json
{
  "success": true,
  "message": "Question created successfully",
  "data": {
    "questionId": 101,
    "content": "Which sign prohibits left turns?"
  },
  "errors": null,
  "meta": null,
  "timestamp": "2026-04-04T10:00:00Z",
  "traceId": "req-001"
}
```

### 10.2. Exam failed because of a critical question
```json
{
  "success": false,
  "message": "Exam failed because a critical question was answered incorrectly",
  "data": null,
  "errors": [
    {
      "code": "CRITICAL_QUESTION_FAILED",
      "field": null,
      "detail": "The candidate answered at least one critical question incorrectly"
    }
  ],
  "meta": null,
  "timestamp": "2026-04-04T10:00:00Z",
  "traceId": "req-002"
}
```

## 11. Message rules

All API messages should be:
- short,
- explicit,
- stable enough for frontend display,
- consistent across similar actions.

Recommended style:
- `Login successful`
- `Question created successfully`
- `Exam submitted successfully`
- `Invalid input data`
- `Resource not found`
- `You do not have permission to access this resource`

## 12. Conclusion

A consistent response standard reduces frontend confusion, improves testability, and makes the whole project feel more professional.
