using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifAi.Auth.RequestContext;
using SpotifAi.Persistence;

namespace SpotifAi.Users.Endpoints;

internal static class GetMeEndpoint
{
    public static RouteGroupBuilder RegisterGetMeEndpoint(this RouteGroupBuilder app)
    {
        app.MapGet("me", GetMe)
            .RequireAuthorization()
            .WithDescription("Returns the current user.")
            .Produces<UnauthorizedResult>(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static async Task<Ok<Response>> GetMe(
        [FromServices] AppDbContext dbContext,
        [FromServices] IRequestContext requestContext,
        CancellationToken cancellationToken
    )
    {
        var user = await dbContext.Users
            .Where(x => x.Id == requestContext.Id)
            .Select(x => new Response(x.Email!,
                    x.UserName!,
                    x.SpotifyAccessToken != null
                )
            )
            .FirstOrDefaultAsync(cancellationToken);

        return TypedResults.Ok(user!);
    }

    public sealed record Response(string Email, string Username, bool ConnectedToSpotify);
}