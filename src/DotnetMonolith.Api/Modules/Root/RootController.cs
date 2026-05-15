using DotnetMonolith.Api.Common.Api;
using DotnetMonolith.Api.Common.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DotnetMonolith.Api.Modules.Root;

[ApiController]
[Route("")]
public sealed class RootController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<RootResponse>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<RootResponse>> Get()
    {
        var response = new RootResponse(
            Name: "DotnetMonolith.Api",
            Status: "running",
            Version: typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.1.0",
            Documentation: "/api/v1/openapi/v1.json");

        return Ok(ApiResponse.Success(response, HttpContext.GetRequestId()));
    }
}

public sealed record RootResponse(
    string Name,
    string Status,
    string Version,
    string Documentation);
