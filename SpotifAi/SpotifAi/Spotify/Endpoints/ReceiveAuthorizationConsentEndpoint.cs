using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpotifAi.Persistence;
using SpotifAi.Spotify.Api.Authorization;
using SpotifAi.Spotify.AuthorizationStateManager;
using SpotifAi.Users;
using SpotifAi.Utils;

namespace SpotifAi.Spotify.Endpoints;

internal static class ReceiveAuthorizationConsentEndpoint
{
    public static RouteGroupBuilder RegisterReceiveAuthorizationConsentEndpoint(this RouteGroupBuilder app)
    {
        app.MapGet("receive-authorization-consent", ReceiveAuthorizationConsent)
            .WithDescription("Receives the authorization consent from the user.");

        return app;
    }

    private static async Task<Results<
        BadRequest<string>,
        RedirectHttpResult
    >> ReceiveAuthorizationConsent(
        [FromQuery] string state,
        [FromQuery] string? code,
        [FromQuery] string? error,
        [FromServices] IOptions<SpotifyConfiguration> spotifyConfiguration,
        [FromServices] IAuthorizationStateManager authorizationStateManager,
        [FromServices] ISpotifyAuthorizationApi spotifyAuthorizationApi,
        [FromServices] AppDbContext dbContext,
        [FromServices] IClock clock
    )
    {
        if (error is not null) return TypedResults.BadRequest(error);

        if (code is null) return TypedResults.BadRequest("No authorization code was provided.");

        // if (!await authorizationStateManager.ValidateStateValueAsync(state, CancellationToken.None))
        //     return TypedResults.BadRequest("Invalid state value.");

        await authorizationStateManager.InvalidateStateValueAsync(state, CancellationToken.None);

        var getAccessTokenResponse = await spotifyAuthorizationApi.GetAccessTokenAsync(code);

        var accessToken = new SpotifyAccessToken
        {
            UserId = Guid.Parse(state),
            AccessToken = getAccessTokenResponse.AccessToken,
            RefreshToken = getAccessTokenResponse.RefreshToken,
            ExpiresAt = clock.Now.AddSeconds(getAccessTokenResponse.ExpiresIn)
        };

        await dbContext.SpotifyAccessTokens.AddAsync(accessToken);
        await dbContext.SaveChangesAsync();

        return TypedResults.Redirect(spotifyConfiguration.Value.RedirectUrl);
    }
}