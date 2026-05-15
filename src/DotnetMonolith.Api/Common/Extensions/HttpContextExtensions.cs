namespace DotnetMonolith.Api.Common.Extensions;

public static class HttpContextExtensions
{
    private const string RequestIdItemKey = "RequestId";

    public static string? GetRequestId(this HttpContext context) =>
        context.Items.TryGetValue(RequestIdItemKey, out var requestId)
            ? requestId?.ToString()
            : context.TraceIdentifier;

    public static void SetRequestId(this HttpContext context, string requestId) =>
        context.Items[RequestIdItemKey] = requestId;
}
