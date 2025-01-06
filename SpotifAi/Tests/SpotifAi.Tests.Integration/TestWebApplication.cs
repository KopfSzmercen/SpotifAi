using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SpotifAi.Persistence;
using SpotifAi.Spotify;
using SpotifAi.Tests.Integration.TestSpotiftApi;

namespace SpotifAi.Tests.Integration;

public class TestWebApplication : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestDatabaseContainer _testDatabaseContainer = new();
    public readonly TestSpotifyApi SpotifyApi = new();

    public async Task InitializeAsync()
    {
        await _testDatabaseContainer.InitializeAsync();
        await SpotifyApi.InitializeAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await SpotifyApi.DisposeAsync();
        await _testDatabaseContainer.DisposeAsync();
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

            services.Remove(
                services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<AppDbContext>))!
            );

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    _testDatabaseContainer.ConnectionString
                );
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(cfg =>
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.IntegrationTests.json")
                .Build();

            cfg.AddConfiguration(configuration);
        });

        return base.CreateHost(builder);
    }
}