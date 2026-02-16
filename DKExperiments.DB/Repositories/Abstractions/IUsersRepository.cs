using DKExperiments.DB.Models.Users;

namespace DKExperiments.DB.Repositories.Abstractions;

public interface IUsersRepository : IRepository<User, UserSearchModel>
{
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    // New overload without timeout parameter to match repository public API
    Task<User?> SaveAsync(User record, CancellationToken cancellationToken);

    // Additional operations used by API layer
    User CreateFromValues(string username, string email, string passwordHash, string? firstName, string? lastName, bool isActive, string? role);

    void UpdateFields(User entity, string username, string email, string? firstName, string? lastName, bool isActive, string? role);

    Task ChangePasswordAsync(Guid id, string newPasswordHash, CancellationToken cancellationToken);

    Task<bool> VerifyPasswordHashAsync(Guid id, string passwordHash, CancellationToken cancellationToken);
}
