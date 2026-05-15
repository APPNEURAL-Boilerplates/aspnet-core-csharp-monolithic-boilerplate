using System.Collections.Concurrent;
using DotnetMonolith.Api.Modules.Users.Entities;

namespace DotnetMonolith.Api.Modules.Users.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public InMemoryUserRepository()
    {
        var seed = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Ada Lovelace",
            Email = "ada@example.com",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        _users.TryAdd(seed.Id, Clone(seed));
    }

    public Task<IReadOnlyCollection<User>> ListAsync(CancellationToken cancellationToken = default)
    {
        var users = _users.Values
            .Where(user => !user.IsDeleted)
            .OrderBy(user => user.CreatedAt)
            .Select(Clone)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<User>>(users);
    }

    public Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var found = _users.TryGetValue(id, out var user) && !user.IsDeleted
            ? Clone(user)
            : null;

        return Task.FromResult(found);
    }

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var found = _users.Values
            .Where(user => !user.IsDeleted)
            .FirstOrDefault(user => user.Email.Equals(normalizedEmail, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(found is null ? null : Clone(found));
    }

    public Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var snapshot = Clone(user);
        _users[snapshot.Id] = snapshot;
        return Task.FromResult(Clone(snapshot));
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _users[user.Id] = Clone(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_users.TryGetValue(id, out var user))
        {
            user.IsDeleted = true;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            _users[id] = Clone(user);
        }

        return Task.CompletedTask;
    }

    private static User Clone(User user) =>
        new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted
        };
}
