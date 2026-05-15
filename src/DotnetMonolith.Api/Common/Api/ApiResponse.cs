namespace DotnetMonolith.Api.Common.Api;

public sealed record ApiResponse<T>(
    bool Ok,
    T? Data,
    ApiError? Error,
    string? RequestId)
{
    public static ApiResponse<T> Success(T data, string? requestId = null) =>
        new(true, data, null, requestId);

    public static ApiResponse<T> Failure(ApiError error, string? requestId = null) =>
        new(false, default, error, requestId);
}

public sealed record ApiError(
    string Code,
    string Message,
    object? Details = null);

public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(T data, string? requestId = null) =>
        ApiResponse<T>.Success(data, requestId);

    public static ApiResponse<object> Failure(
        string code,
        string message,
        object? details = null,
        string? requestId = null) =>
        ApiResponse<object>.Failure(new ApiError(code, message, details), requestId);
}
