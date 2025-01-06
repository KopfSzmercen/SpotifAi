using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpotifAi.Spotify;
using SpotifAi.Tests.Integration.TestSpotiftApi;

namespace SpotifAi.Tests.Integration;

public class TestWebApplication : WebApplicationFactory<Program>, IAsyncLifetime
{
    public readonly TestSpotifyApi SpotifyApi = new();

    public async Task InitializeAsync()
    {
        await SpotifyApi.InitializeAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await SpotifyApi.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IConfigureOptions<SpotifyConfiguration>));

            if (descriptor != null) services.Remove(descriptor);

            services.Configure<SpotifyConfiguration>(options =>
            {
                options.Scope = DefaultTestSpotifyConfiguration.SpotifyConfiguration.Scope;
                options.ClientId = DefaultTestSpotifyConfiguration.SpotifyConfiguration.ClientId;
                options.ClientSecret = DefaultTestSpotifyConfiguration.SpotifyConfiguration.ClientSecret;
                options.RedirectUrl = DefaultTestSpotifyConfiguration.SpotifyConfiguration.RedirectUrl;
                options.SpotifyAuthorizationUrl = $"{SpotifyApi.BaseAddress}/authorize";
                options.SpotifyTokenUrl = $"{SpotifyApi.BaseAddress}/api/token";
            });
        });
    }
}