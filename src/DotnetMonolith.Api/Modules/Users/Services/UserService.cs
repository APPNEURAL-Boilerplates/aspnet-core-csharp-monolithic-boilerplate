using DotnetMonolith.Api.Common.Exceptions;
using DotnetMonolith.Api.Modules.Users.Dtos;
using DotnetMonolith.Api.Modules.Users.Entities;
using DotnetMonolith.Api.Modules.Users.Repositories;

namespace DotnetMonolith.Api.Modules.Users.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users) => _users = users;

    public async Task<IReadOnlyCollection<UserResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var users = await _users.ListAsync(cancellationToken);
        return users.Select(ToResponse).ToArray();
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _users.FindByIdAsync(id, cancellationToken)
            ?? throw AppException.NotFound("User", id);

        return ToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var existing = await _users.FindByEmailAsync(email, cancellationToken);

        if (existing is not null)
        {
            throw AppException.Conflict("USER_EMAIL_ALREADY_EXISTS", "A user with this email already exists.");
        }

        var now = DateTimeOffset.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = email,
            CreatedAt = now,
            UpdatedAt = now
        };

        var created = await _users.AddAsync(user, cancellationToken);
        return ToResponse(created);
    }

    public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.FindByIdAsync(id, cancellationToken)
            ?? throw AppException.NotFound("User", id);

        var hasChanges = false;

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name.Trim();
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = NormalizeEmail(request.Email);
            var existing = await _users.FindByEmailAsync(email, cancellationToken);

            if (existing is not null && existing.Id != id)
            {
                throw AppException.Conflict("USER_EMAIL_ALREADY_EXISTS", "A user with this email already exists.");
            }

            user.Email = email;
            hasChanges = true;
        }

        if (!hasChanges)
        {
            throw AppException.BadRequest("NO_FIELDS_TO_UPDATE", "Provide at least one field to update.");
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _users.UpdateAsync(user, cancellationToken);

        return ToResponse(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _ = await _users.FindByIdAsync(id, cancellationToken)
            ?? throw AppException.NotFound("User", id);

        await _users.DeleteAsync(id, cancellationToken);
    }

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    private static UserResponse ToResponse(User user) =>
        new(user.Id, user.Name, user.Email, user.CreatedAt, user.UpdatedAt);
}
