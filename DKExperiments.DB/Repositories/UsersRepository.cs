using DKExperiments.DB.Models.Users;
using DKExperiments.DB.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DKExperiments.DB.Repositories;

public class UsersRepository(DKDBContext dbContext) : ResilientRepository, IUsersRepository
{
	public Task<User?> GetAsync(UserSearchModel searchModel, CancellationToken cancellationToken) =>
		dbContext.Set<User>().FirstOrDefaultAsync(x =>
			(!searchModel.Id.HasValue || x.Id == searchModel.Id.Value)
			&& (string.IsNullOrWhiteSpace(searchModel.Username) || x.Username == searchModel.Username)
			&& (string.IsNullOrWhiteSpace(searchModel.Email) || x.Email == searchModel.Email),
			cancellationToken);

	public Task<List<User>> ListAsync(UserSearchModel searchModel, CancellationToken cancellationToken) =>
		dbContext.Set<User>().Where(x =>
			(!searchModel.Id.HasValue || x.Id == searchModel.Id.Value)
			&& (string.IsNullOrWhiteSpace(searchModel.Username) || x.Username == searchModel.Username)
			&& (string.IsNullOrWhiteSpace(searchModel.Email) || x.Email == searchModel.Email)
		).ToListAsync(cancellationToken);

	public Task DeleteAsync(Guid id, CancellationToken cancellationToken) =>
		dbContext.Set<User>().Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);

	public async Task<User?> SaveAsync(User record, CancellationToken cancellationToken)
	{
		var item = await GetAsync(new UserSearchModel(record.Id, record.Username, record.Email), cancellationToken);

		var create = item == null;

		item ??= new User();

		// copy editable fields
		item.Username = record.Username;
		item.Email = record.Email;
		item.PasswordHash = record.PasswordHash;
		item.FirstName = record.FirstName;
		item.LastName = record.LastName;
		item.IsActive = record.IsActive;
		item.Role = record.Role;

		item.UpdatedAt = DateTime.UtcNow;

		if (create)
		{
			item.CreatedAt = DateTime.UtcNow;
			dbContext.Set<User>().Add(item);
		}
		else
		{
			dbContext.Set<User>().Update(item);
		}

		await dbContext.SaveChangesResilientAsync(x => x.Entity is User entity && entity.Id == record.Id, cancellationToken);

		return item;
	}

	// Explicit interface implementation to keep compatibility with the generic IRepository signature
	async Task<User?> IRepository<User, UserSearchModel>.SaveAsync(User record, bool timeout, CancellationToken cancellationToken)
	{
		return await SaveAsync(record, cancellationToken);
	}

	/// <summary>
	/// Create a new user entity from provided values and hashed password
	/// </summary>
	public User CreateFromValues(string username, string email, string passwordHash, string? firstName, string? lastName, bool isActive, string? role)
	{
		return new User
		{
			Id = Guid.NewGuid(),
			Username = username,
			Email = email,
			PasswordHash = passwordHash,
			FirstName = firstName,
			LastName = lastName,
			IsActive = isActive,
			Role = role,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow
		};
	}

	/// <summary>
	/// Update user fields except password
	/// </summary>
	public void UpdateFields(User entity, string username, string email, string? firstName, string? lastName, bool isActive, string? role)
	{
		entity.Username = username;
		entity.Email = email;
		entity.FirstName = firstName;
		entity.LastName = lastName;
		entity.IsActive = isActive;
		entity.Role = role;
		entity.UpdatedAt = DateTime.UtcNow;
	}

	/// <summary>
	/// Change password hash and save
	/// </summary>
	public async Task ChangePasswordAsync(Guid id, string newPasswordHash, CancellationToken cancellationToken)
	{
		var user = await GetAsync(new UserSearchModel(id, null, null), cancellationToken);
		if (user == null) throw new InvalidOperationException("User not found");
		user.PasswordHash = newPasswordHash;
		user.UpdatedAt = DateTime.UtcNow;
		dbContext.Set<User>().Update(user);
		await dbContext.SaveChangesResilientAsync(x => x.Entity is User e && e.Id == id, cancellationToken);
	}

	/// <summary>
	/// Verify password hash equality - repository doesn't perform hashing itself
	/// </summary>
	public Task<bool> VerifyPasswordHashAsync(Guid id, string passwordHash, CancellationToken cancellationToken) =>
		GetAsync(new UserSearchModel(id, null, null), cancellationToken).ContinueWith(t => t.Result != null && t.Result.PasswordHash == passwordHash, cancellationToken);
}
