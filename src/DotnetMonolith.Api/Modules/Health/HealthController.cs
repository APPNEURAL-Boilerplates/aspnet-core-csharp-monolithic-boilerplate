using DotnetMonolith.Api.Common.Api;
using DotnetMonolith.Api.Common.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DotnetMonolith.Api.Modules.Health;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<HealthResponse>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<HealthResponse>> Get()
    {
        var response = new HealthResponse(
            Status: "healthy",
            Timestamp: DateTimeOffset.UtcNow);

        return Ok(ApiResponse.Success(response, HttpContext.GetRequestId()));
    }
}

public sealed record HealthResponse(
    string Status,
    DateTimeOffset Timestamp);
