using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpotifAi.Auth.RequestContext;
using SpotifAi.Spotify.AuthorizationStateManager;

namespace SpotifAi.Spotify.Endpoints;

internal static class InitializeSpotifyAuthorizationEndpoint
{
    public static RouteGroupBuilder RegisterInitializeSpotifyAuthorizationEndpoint(this RouteGroupBuilder app)
    {
        app.MapGet("initialize-authorization", GetAuthorizationUrl)
            .RequireAuthorization()
            .WithDescription("Initializes the Spotify authorization process.");

        return app;
    }

    private static async Task<ContentHttpResult> GetAuthorizationUrl(
        [FromServices] IOptions<SpotifyConfiguration> spotifyConfiguration,
        [FromServices] IAuthorizationStateManager authorizationStateManager,
        [FromServices] IRequestContext requestContext,
        CancellationToken cancellationToken
    )
    {
        var state = requestContext.Id;

        await authorizationStateManager.StoreStateValueAsync(state.ToString()!, cancellationToken);

        var query = new Dictionary<string, string>
        {
            { "client_id", spotifyConfiguration.Value.ClientId },
            { "response_type", "code" },
            { "redirect_uri", spotifyConfiguration.Value.RedirectUrl },
            { "scope", spotifyConfiguration.Value.Scope },
            { "state", state.ToString()! }
        };

        var queryString = string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));

        return TypedResults.Text($"{spotifyConfiguration.Value.SpotifyAuthorizationUrl}?{queryString}");
    }
}