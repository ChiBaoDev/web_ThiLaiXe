using System.Text.Json.Serialization;

namespace HeThongThiBangLai.Api.Common.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<ApiError>? Errors { get; set; }
    public PaginationMeta? Meta { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }

    public ApiResponse()
    {
    }

    public ApiResponse(bool success, string message, T? data = default, List<ApiError>? errors = null, PaginationMeta? meta = null)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors;
        Meta = meta;
        Timestamp = DateTime.UtcNow;
    }
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
