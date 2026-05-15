using DotnetMonolith.Api.Common.Api;
using DotnetMonolith.Api.Common.Exceptions;
using DotnetMonolith.Api.Common.Extensions;

namespace DotnetMonolith.Api.Common.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogError(exception, "Unhandled exception after response started.");
            throw exception;
        }

        var statusCode = StatusCodes.Status500InternalServerError;
        var code = "INTERNAL_SERVER_ERROR";
        var message = "An unexpected error occurred.";
        object? details = null;

        if (exception is AppException appException)
        {
            statusCode = appException.StatusCode;
            code = appException.Code;
            message = appException.Message;
            details = appException.Details;
        }

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception: {ErrorCode}", code);
        }
        else
        {
            _logger.LogWarning(exception, "Handled application exception: {ErrorCode}", code);
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(
            ApiResponse.Failure(code, message, details, context.GetRequestId()));
    }
}
