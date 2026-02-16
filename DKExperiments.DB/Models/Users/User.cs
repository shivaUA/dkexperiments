namespace DKExperiments.DB.Models.Users;

/// <summary>
/// Application user entity
/// </summary>
public class User
{
	public Guid Id { get; set; }

	public string Username { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// Hashed password
	/// </summary>
	public string PasswordHash { get; set; } = string.Empty;

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public bool IsActive { get; set; } = true;

	public string? Role { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }
}
