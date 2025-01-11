using SpotifAi.Spotify.Api;
using SpotifAi.Spotify.Api.Authorization;
using SpotifAi.Spotify.AuthorizationStateManager;
using SpotifAi.Spotify.Endpoints;

namespace SpotifAi.Spotify;

internal static class SpotifyModule
{
    public static IServiceCollection AddSpotifyModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SpotifyConfiguration>(configuration.GetSection(SpotifyConfiguration.SectionName));

        services.AddSingleton<IAuthorizationStateManager, InMemoryAuthorizationStateManager>();

        services.AddTransient<ApplyAuthorizationApiDefaultHeadersDelegatingHandler>();

        services
            .AddHttpClient<ISpotifyAuthorizationApi, SpotifyAuthorizationApi>()
            .AddHttpMessageHandler<ApplyAuthorizationApiDefaultHeadersDelegatingHandler>()
            .AddStandardResilienceHandler();


        services.AddTransient<AttachUserAccessTokenDelegatingHandler>();

        services
            .AddHttpClient<SendSpotifyRequest>()
            .AddHttpMessageHandler<AttachUserAccessTokenDelegatingHandler>()
            .AddStandardResilienceHandler();

        return services;
    }

    public static IApplicationBuilder UseSpotifyModule(this WebApplication app)
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