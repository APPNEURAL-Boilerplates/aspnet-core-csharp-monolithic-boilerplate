using DotnetMonolith.Api.Modules.Users.Dtos;

namespace DotnetMonolith.Api.Modules.Users.Services;

public interface IUserService
{
    Task<IReadOnlyCollection<UserResponse>> ListAsync(CancellationToken cancellationToken = default);

    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
