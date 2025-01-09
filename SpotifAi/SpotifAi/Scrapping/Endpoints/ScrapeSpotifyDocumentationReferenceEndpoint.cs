using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SpotifAi.Ai;
using SpotifAi.Ai.Assistants.SpotifyDocumentation;

namespace SpotifAi.Scrapping.Endpoints;

internal static class ScrapeSpotifyDocumentationReferenceEndpoint
{
    private const string SpotifyDocumentationUrl = "https://developer.spotify.com/documentation/web-api";

    public static RouteGroupBuilder RegisterScrapeSpotifyDocumentationReferenceEndpoint(this RouteGroupBuilder app)
    {
        app.MapPost("spotify-documentation-reference", ScrapeUrl)
            //TODO - Add authorization policy
            //TODO - Make this a queueable task instead of waiting for the response
            .WithDescription("Scrape the Spotify documentation reference page and return the markdown content.");

        return app;
    }

    private static async Task<Ok<string>> ScrapeUrl(
        [FromServices] IScrappingService scrappingService,
        [FromServices] IAi ai,
        [FromServices] SpotifyDocumentationPartSelectionAssistant assistant,
        CancellationToken cancellationToken
    )
    {
        var markdown = await scrappingService.GetMarkdownAsync(SpotifyDocumentationUrl, cancellationToken);

        var path = Path.Combine("SpotifyDocumentation", "spotify-documentation-reference.md");

        var structuredApiReference = await assistant.SelectPartAsync(markdown, cancellationToken);

        await File.WriteAllTextAsync(path, structuredApiReference, cancellationToken);

        return TypedResults.Ok(markdown);
    }
}