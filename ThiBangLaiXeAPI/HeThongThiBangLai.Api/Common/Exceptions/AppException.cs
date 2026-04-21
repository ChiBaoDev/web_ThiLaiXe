using HeThongThiBangLai.Api.Common.Responses;

namespace HeThongThiBangLai.Api.Common.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }
    public List<ApiError>? Errors { get; }

    public AppException(
        string message,
        string errorCode = "SERVER_ERROR",
        int statusCode = 500,
        List<ApiError>? errors = null)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        Errors = errors;
    }
}

public class ValidationAppException : AppException
{
    public ValidationAppException(List<ApiError> errors)
        : base("Validation failed", "VALIDATION_ERROR", 400, errors)
    {
    }
}

public class NotFoundAppException : AppException
{
    public NotFoundAppException(string message = "Resource not found", string errorCode = "NOT_FOUND")
        : base(message, errorCode, 404)
    {
    }
}

public class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string message = "You do not have permission to access this resource", string errorCode = "FORBIDDEN")
        : base(message, errorCode, 403)
    {
    }
}

public class BusinessRuleAppException : AppException
{
    public BusinessRuleAppException(string message, string errorCode = "BUSINESS_RULE_VIOLATION", int statusCode = 422, List<ApiError>? errors = null)
        : base(message, errorCode, statusCode, errors)
    {
    }
}

public class ConflictAppException : AppException
{
    public ConflictAppException(string message, string errorCode = "CONFLICT", List<ApiError>? errors = null)
        : base(message, errorCode, 409, errors)
    {
    }
}
