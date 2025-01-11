using Microsoft.Extensions.Options;
using SpotifAi.Scrapping.Endpoints;
using SpotifAi.Scrapping.Firecrawl;

namespace SpotifAi.Scrapping;

internal static class ScrappingModule
{
    public static IServiceCollection AddScrappingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FirecrawlConfiguration>(configuration.GetSection(FirecrawlConfiguration.SectionName));

        services.AddTransient<FirecrawlAuthorizationDelegatingHandler>();

        services.AddScoped<IScrappingService, FirecrawlService>();

        services
            .AddHttpClient<IScrappingService, FirecrawlService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<FirecrawlConfiguration>>().Value;

                client.BaseAddress = new Uri(config.BaseUrl);
            })
            .AddHttpMessageHandler<FirecrawlAuthorizationDelegatingHandler>()
            .AddStandardResilienceHandler();

        services.AddSingleton<SpotifyDocumentationScrapping>();

        return services;
    }

    public static IApplicationBuilder UseScrappingModule(this WebApplication app)
    {
        var group = app
            .MapGroup("scrapping")
            .WithTags("Scrapping");

        group.RegisterPerformEndpointSelectionAndPreparationWorkflowEndpoint();
        return app;
    }
}