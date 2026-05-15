using DotnetMonolith.Api.Modules.Users.Entities;

namespace DotnetMonolith.Api.Modules.Users.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyCollection<User>> ListAsync(CancellationToken cancellationToken = default);

    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
