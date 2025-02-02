using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpotifAi.Auth.Tokens;

namespace SpotifAi.Users.Endpoints;

internal static class SignInJwtEndpoint
{
    public static RouteGroupBuilder RegisterSignInJwtEndpoint(this RouteGroupBuilder app)
    {
        app.MapPost("sign-in-jwt", Handle)
            .WithDescription("Signs in a user and returns a JWT.");

        return app;
    }

    private static async Task<
        Results<
            BadRequest<string>,
            Ok<Response>
        >> Handle(
        [FromServices] SignInManager<User> signInManager,
        [FromServices] UserManager<User> userManager,
        [FromServices] ITokensManager tokensManager,
        [FromBody] Reuqest request
    )
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return TypedResults.BadRequest("Invalid credentials.");

        var signInResult = await signInManager
            .PasswordSignInAsync(user, request.Password, false, false);

        if (signInResult.IsLockedOut)
            return TypedResults.BadRequest("Account is locked out.");

        if (!signInResult.Succeeded)
            return TypedResults.BadRequest("Invalid credentials.");

        var userRoles = await userManager.GetRolesAsync(user);

        var userClaims = await userManager.GetClaimsAsync(user);

        foreach (var role in userRoles) userClaims.Add(new Claim(ClaimTypes.Role, role));

        var token = tokensManager.CreateToken(
            user.Id,
            userRoles.ToList(),
            [
                ..userClaims.ToList()
            ]
        );

        return TypedResults.Ok(new Response(token.AccessToken));
    }

    public sealed record Response(string Token);

    public sealed record Reuqest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}