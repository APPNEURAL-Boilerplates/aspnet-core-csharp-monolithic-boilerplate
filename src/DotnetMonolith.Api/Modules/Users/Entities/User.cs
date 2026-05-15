namespace DotnetMonolith.Api.Modules.Users.Entities;

public sealed class User
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Name { get; set; }

    public required string Email { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public bool IsDeleted { get; set; }
}
