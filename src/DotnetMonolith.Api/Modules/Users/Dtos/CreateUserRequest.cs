using System.ComponentModel.DataAnnotations;

namespace DotnetMonolith.Api.Modules.Users.Dtos;

public sealed class CreateUserRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; init; } = string.Empty;
}
