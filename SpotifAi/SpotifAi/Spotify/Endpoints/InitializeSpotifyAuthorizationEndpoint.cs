using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpotifAi.Spotify.AuthorizationStateManager;

namespace SpotifAi.Spotify.Endpoints;

internal static class InitializeSpotifyAuthorizationEndpoint
{
    public static RouteGroupBuilder RegisterInitializeSpotifyAuthorizationEndpoint(this RouteGroupBuilder app)
    {
        app.MapPost("initialize-authorization", GetAuthorizationUrl)
            .WithDescription("Initializes the Spotify authorization process.");

        return app;
    }

    private static async Task<RedirectHttpResult> GetAuthorizationUrl(
        [FromServices] IOptions<SpotifyConfiguration> spotifyConfiguration,
        [FromServices] IAuthorizationStateManager authorizationStateManager,
        CancellationToken cancellationToken
    )
    {
        var state = authorizationStateManager.GenerateRandomStateValue();

        await authorizationStateManager.StoreStateValueAsync(state, cancellationToken);

        var query = new Dictionary<string, string>
        {
            { "client_id", spotifyConfiguration.Value.ClientId },
            { "response_type", "code" },
            { "redirect_uri", spotifyConfiguration.Value.RedirectUrl },
            { "scope", spotifyConfiguration.Value.Scope },
            { "state", state }
        };

        var queryString = string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));

        return TypedResults.Redirect($"{spotifyConfiguration.Value.SpotifyAuthorizationUrl}?{queryString}");
    }
}