using DKExperiments.API.Models.Users;
using DKExperiments.DB.Models.Users;

namespace DKExperiments.API.Endpoints;

internal static class UsersMapper
{
    public static UserModel MapToApi(User u) => new UserModel
    {
        Id = u.Id,
        Username = u.Username,
        Email = u.Email,
        FirstName = u.FirstName,
        LastName = u.LastName,
        IsActive = u.IsActive,
        Role = u.Role,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };
}
