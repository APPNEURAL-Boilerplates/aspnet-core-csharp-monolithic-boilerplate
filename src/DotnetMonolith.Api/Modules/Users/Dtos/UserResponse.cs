namespace DotnetMonolith.Api.Modules.Users.Dtos;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
