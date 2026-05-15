using DotnetMonolith.Api.Common.Extensions;

namespace DotnetMonolith.Api.Common.Middleware;

public sealed class RequestIdMiddleware
{
    private const string HeaderName = "X-Request-ID";
    private readonly RequestDelegate _next;

    public RequestIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers.TryGetValue(HeaderName, out var headerValue)
            && !string.IsNullOrWhiteSpace(headerValue.ToString())
                ? headerValue.ToString()
                : context.TraceIdentifier;

        context.SetRequestId(requestId);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = requestId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
