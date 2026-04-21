using System.Text.Json;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace HeThongThiBangLai.Api.Common.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var traceId = context.TraceIdentifier;
        ApiResponse<object> response;

        switch (exception)
        {
            case AppException appEx:
                context.Response.StatusCode = appEx.StatusCode;
                response = new ApiResponse<object>
                {
                    Success = false,
                    Message = appEx.Message,
                    Errors = appEx.Errors ?? new List<ApiError> { new ApiError { Code = appEx.ErrorCode, Detail = appEx.Message } },
                    TraceId = traceId
                };
                break;

            case UnauthorizedAccessException unauthorizedEx:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new ApiResponse<object>
                {
                    Success = false,
                    Message = unauthorizedEx.Message,
                    Errors = new List<ApiError> { new ApiError { Code = "UNAUTHORIZED", Detail = unauthorizedEx.Message } },
                    TraceId = traceId
                };
                break;

            default:
                _logger.LogError(exception, "Unexpected error occurred. TraceId: {TraceId}", traceId);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new ApiResponse<object>
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Errors = new List<ApiError> { new ApiError { Code = "SERVER_ERROR", Detail = "An unexpected error occurred" } },
                    TraceId = traceId
                };
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
