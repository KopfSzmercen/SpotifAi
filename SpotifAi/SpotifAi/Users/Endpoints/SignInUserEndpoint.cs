using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SpotifAi.Users.Endpoints;

internal static class SignInUserEndpoint
{
    public static RouteGroupBuilder RegisterSignInUserEndpoint(this RouteGroupBuilder app)
    {
        app.MapPost("sign-in", SignInUser)
            .WithDescription("Signs in a user.");

        return app;
    }

    private static async Task<
        Results<
            BadRequest<string>,
            Ok>
    > SignInUser(
        [FromServices] SignInManager<User> signInManager,
        [FromServices] UserManager<User> userManager,
        [FromBody] Request request
    )
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return TypedResults.BadRequest("Invalid credentials.");

        var signInResult = await signInManager
            .PasswordSignInAsync(user, request.Password, true, false);

        if (signInResult.IsLockedOut)
            return TypedResults.BadRequest("Account is locked out.");

        if (!signInResult.Succeeded)
            return TypedResults.BadRequest(" Invalid credentials.");

        return TypedResults.Ok();
    }

    public sealed record Request
    {
        public string Email { get; init; }
        public string Password { get; init; }
    }
}