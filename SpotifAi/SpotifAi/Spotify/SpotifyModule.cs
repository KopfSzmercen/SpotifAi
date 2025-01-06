using SpotifAi.Spotify.Api.Authorization;
using SpotifAi.Spotify.AuthorizationStateManager;
using SpotifAi.Spotify.Endpoints;

namespace SpotifAi.Spotify;

internal static class SpotifyModule
{
    public static IServiceCollection AddSpotify(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SpotifyConfiguration>(configuration.GetSection(SpotifyConfiguration.SectionName));

        services.AddSingleton<IAuthorizationStateManager, InMemoryAuthorizationStateManager>();

        services.AddTransient<ApplyAuthorizationApiDefaultHeadersDelegatingHandler>();

        services
            .AddHttpClient<ISpotifyAuthorizationApi, SpotifyAuthorizationApi>()
            .AddHttpMessageHandler<ApplyAuthorizationApiDefaultHeadersDelegatingHandler>();

        return services;
    }

    public static IApplicationBuilder UseSpotify(this WebApplication app)
    {
        var group = app
            .MapGroup("spotify")
            .WithTags("Spotify");

        group
            .RegisterInitializeSpotifyAuthorizationEndpoint()
            .RegisterReceiveAuthorizationConsentEndpoint();

        return app;
    }
}