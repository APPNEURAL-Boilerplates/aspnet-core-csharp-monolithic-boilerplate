using System.ComponentModel.DataAnnotations;

namespace DotnetMonolith.Api.Modules.Users.Dtos;

public sealed class UpdateUserRequest
{
    [MinLength(2)]
    [MaxLength(100)]
    public string? Name { get; init; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; init; }
}
