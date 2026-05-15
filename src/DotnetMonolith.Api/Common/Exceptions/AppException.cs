namespace DotnetMonolith.Api.Common.Exceptions;

public class AppException : Exception
{
    public AppException(int statusCode, string code, string message, object? details = null)
        : base(message)
    {
        StatusCode = statusCode;
        Code = code;
        Details = details;
    }

    public int StatusCode { get; }

    public string Code { get; }

    public object? Details { get; }

    public static AppException BadRequest(string code, string message, object? details = null) =>
        new(StatusCodes.Status400BadRequest, code, message, details);

    public static AppException NotFound(string resource, object id) =>
        new(StatusCodes.Status404NotFound, "NOT_FOUND", $"{resource} '{id}' was not found.");

    public static AppException Conflict(string code, string message, object? details = null) =>
        new(StatusCodes.Status409Conflict, code, message, details);
}
