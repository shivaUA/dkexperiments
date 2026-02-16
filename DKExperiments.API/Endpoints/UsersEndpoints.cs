using DKExperiments.API.Models.Users;
using DKExperiments.DB.Models.Users;
using DKExperiments.DB.Repositories.Abstractions;
using static DKExperiments.API.Endpoints.UsersMapper;

namespace DKExperiments.API.Endpoints;

internal static class UsersEndpoints
{
	public static RouteGroupBuilder MapUserRoutes(this RouteGroupBuilder routes)
	{
        routes.MapGet("/", GetUsers)
            .WithName("Get users list")
            .Produces<List<UserModel>>(StatusCodes.Status200OK);

        routes.MapGet("/{id}", GetUserById)
            .WithName("Get user by id")
            .Produces<UserModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        routes.MapPost("/", CreateUser)
            .WithName("Create user")
            .Produces<UserModel>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        routes.MapPut("/{id}", UpdateUser)
            .WithName("Update user")
            .Produces<UserModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

		routes.MapDelete("/{id}", DeleteUser)
			.WithName("Delete user")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status404NotFound);

        // Password change endpoint
        routes.MapPost("/{id}/password", ChangePassword)
            .WithName("Change password")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return routes;
	}

    public static async Task<IResult> GetUsers(IUsersRepository usersRepository, CancellationToken cancellationToken)
    {
        var users = await usersRepository.ListAsync(new UserSearchModel(null, null, null), cancellationToken);
        return Results.Ok(users.Select(MapToApi).ToList());
    }

    public static async Task<IResult> GetUserById(IUsersRepository usersRepository, Guid id, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetAsync(new UserSearchModel(id, null, null), cancellationToken);
        return user == null ? Results.NotFound() : Results.Ok(MapToApi(user));
    }

    public static async Task<IResult> CreateUser(IUsersRepository usersRepository, CreateUserModel model, CancellationToken cancellationToken)
    {
        // Very naive password hashing for demo purposes
        var passwordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.Password));

        var entity = usersRepository.CreateFromValues(model.Username, model.Email, passwordHash, model.FirstName, model.LastName, model.IsActive, model.Role);

        var created = await usersRepository.SaveAsync(entity, cancellationToken);
        return Results.Created($"/users/{created.Id}", MapToApi(created));
    }

    public static async Task<IResult> UpdateUser(IUsersRepository usersRepository, Guid id, UpdateUserModel model, CancellationToken cancellationToken)
    {
        var exists = await usersRepository.GetAsync(new UserSearchModel(id, null, null), cancellationToken);
        if (exists == null) return Results.NotFound();

        usersRepository.UpdateFields(exists, model.Username, model.Email, model.FirstName, model.LastName, model.IsActive, model.Role);
        var updated = await usersRepository.SaveAsync(exists, cancellationToken);
        return Results.Ok(MapToApi(updated));
    }

	public static async Task<IResult> DeleteUser(IUsersRepository usersRepository, Guid id, CancellationToken cancellationToken)
	{
		var exists = await usersRepository.GetAsync(new UserSearchModel(id, null, null), cancellationToken);
		if (exists == null) return Results.NotFound();

		await usersRepository.DeleteAsync(id, cancellationToken);
		return Results.NoContent();
	}

    public static async Task<IResult> ChangePassword(IUsersRepository usersRepository, Guid id, ChangePasswordModel model, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetAsync(new UserSearchModel(id, null, null), cancellationToken);
        if (user == null) return Results.NotFound();

        // naive check: compare base64 encoding
        var currentHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.CurrentPassword));
        if (!await usersRepository.VerifyPasswordHashAsync(id, currentHash, cancellationToken))
        {
            return Results.BadRequest();
        }

        var newHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.NewPassword));
        await usersRepository.ChangePasswordAsync(id, newHash, cancellationToken);
        return Results.NoContent();
    }

    
}
