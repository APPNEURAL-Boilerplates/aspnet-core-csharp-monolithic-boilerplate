using System.Text.Json;
using System.Text.Json.Serialization;
using DotnetMonolith.Api.Common.Api;
using DotnetMonolith.Api.Common.Extensions;
using DotnetMonolith.Api.Common.Middleware;
using DotnetMonolith.Api.Modules.Users.Repositories;
using DotnetMonolith.Api.Modules.Users.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options =>
    {
        options.ReturnHttpNotAcceptable = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

            var response = ApiResponse.Failure(
                code: "VALIDATION_ERROR",
                message: "One or more validation errors occurred.",
                details: errors,
                requestId: context.HttpContext.GetRequestId());

            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSecurityHeaders();

app.UseStatusCodePages(async statusCodeContext =>
{
    var httpContext = statusCodeContext.HttpContext;

    if (httpContext.Response.HasStarted || httpContext.Response.ContentLength.HasValue)
    {
        return;
    }

    var statusCode = httpContext.Response.StatusCode;
    var (code, message) = statusCode switch
    {
        StatusCodes.Status404NotFound => ("NOT_FOUND", "The requested endpoint was not found."),
        StatusCodes.Status405MethodNotAllowed => ("METHOD_NOT_ALLOWED", "The HTTP method is not allowed for this endpoint."),
        _ => ($"HTTP_{statusCode}", "The request could not be completed.")
    };

    httpContext.Response.ContentType = "application/json";

    await httpContext.Response.WriteAsJsonAsync(
        ApiResponse.Failure(code, message, requestId: httpContext.GetRequestId()));
});

app.UseRouting();
app.UseCors("DefaultCors");
app.UseAuthorization();

if (app.Configuration.GetValue("OpenApi:Enabled", true))
{
    app.MapOpenApi("/api/v1/openapi/{documentName}.json");
}

app.MapHealthChecks("/actuator/health");
app.MapControllers();

app.Run();

public partial class Program;
