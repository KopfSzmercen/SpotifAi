using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpotifAi.Persistence;

namespace SpotifAi.Users.Endpoints;

internal static class RegisterUserEndpoint
{
    public static RouteGroupBuilder RegisterRegisterUserEndpoint(this RouteGroupBuilder app)
    {
        app.MapPost("register", RegisterUser)
            .WithDescription("Registers a new user.");

        return app;
    }

    private static async Task<
        Results
        <BadRequest<string>, Ok<Guid>
        >
    > RegisterUser(
        [FromServices] UserManager<User> userManager,
        [FromServices] AppDbContext dbContext,
        [FromBody] Request request
    )
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var (success, error, userId) = await TryRegisterUser(request, userManager);

        if (!success)
        {
            await transaction.RollbackAsync();
            return TypedResults.BadRequest(error!);
        }

        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return TypedResults.Ok((Guid)userId!);
    }

    private static async Task<(bool, string?, Guid?)> TryRegisterUser(Request request,
        UserManager<User> userManager)
    {
        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            Id = Guid.NewGuid()
        };

        var createUserResult = await userManager.CreateAsync(user, request.Password);

        if (!createUserResult.Succeeded)
        {
            var error = createUserResult.Errors
                .First();

            return (false, error.Description, null);
        }

        await userManager.AddToRolesAsync(user, [UserRole.User]);

        await userManager.AddClaimsAsync(user, [
            new Claim("UserId", user.Id.ToString())
        ]);

        return (true, null, user.Id);
    }

    public sealed record Request
    {
        public string Email { get; init; }
        public string Password { get; init; }
    }
}