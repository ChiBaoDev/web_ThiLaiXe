using System.Collections.Generic;
using HeThongThiBangLai.Api.Common.Responses;

namespace HeThongThiBangLai.Api.Common.Responses;

public static class ApiResponseFactory
{
    public static ApiResponse<T> Success<T>(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Created<T>(T data, string message = "Created successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<object> Fail(string message, List<ApiError>? errors = null)
    {
        return new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    public static ApiResponse<T> Fail<T>(string message, List<ApiError>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    public static ApiResponse<PagedList<TItem>> SuccessPaged<TItem>(PagedList<TItem> pagedData, string message = "Success")
    {
        var response = new ApiResponse<PagedList<TItem>>
        {
            Success = true,
            Message = message,
            Data = pagedData
        };
        response.Meta = new PaginationMeta
        {
            Page = pagedData.Page,
            PageSize = pagedData.PageSize,
            TotalItems = pagedData.TotalCount,
            TotalPages = pagedData.TotalPages
        };
        return response;
    }

    public static ApiResponse<object> Error(string message, string errorCode = "SERVER_ERROR")
    {
        return new ApiResponse<object>
        {
            Success = false,
            Message = message,
            Errors = new List<ApiError> { new ApiError { Code = errorCode, Detail = message } }
        };
    }
}
