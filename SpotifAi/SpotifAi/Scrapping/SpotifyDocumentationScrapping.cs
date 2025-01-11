using SpotifAi.Ai.Assistants.SpotifyDocumentation;

namespace SpotifAi.Scrapping;

internal sealed class SpotifyDocumentationScrapping(
    IScrappingService scrappingService,
    SpotifyDocumentationReferenceSelectionAssistant spotifyDocumentationReferenceSelectionAssistant,
    SpotifyEndpointParametersSelectionAssistant spotifyEndpointParametersSelectionAssistant
)
{
    private const string FolderName = "SpotifyDocumentation";
    private const string RelevantContentStart = "## Request";
    private const string RelevantContentEnd = "## Response";
    private const string DocumentationMainPage = "https://developer.spotify.com/documentation/web-api";

    public async Task<string> GetSpotifyEndpointDetailsAsync(string endpoint, CancellationToken cancellationToken)
    {
        var endpointForPath = endpoint.Replace("/", "-").Replace("\\", "-");
        var path = Path.Combine(FolderName, $"{endpointForPath}.md");

        var isAlreadyScrapped = File.Exists(path);

        if (isAlreadyScrapped)
            return await File.ReadAllTextAsync(path, cancellationToken);

        var scrapedSelectedEndpoint =
            await scrappingService.GetMarkdownAsync(
                $"https://developer.spotify.com/{endpoint}",
                cancellationToken);

        var startIndex = scrapedSelectedEndpoint.IndexOf(RelevantContentStart, StringComparison.OrdinalIgnoreCase) +
                         RelevantContentStart.Length;

        var endIndex = scrapedSelectedEndpoint.IndexOf(RelevantContentEnd, StringComparison.OrdinalIgnoreCase);

        var endpointDetails = scrapedSelectedEndpoint[startIndex..endIndex];

        var extractedAndStructuredEndpointDetails = await spotifyEndpointParametersSelectionAssistant.SelectPartAsync(
            endpointDetails,
            cancellationToken
        );

        await File.WriteAllTextAsync(
            path,
            extractedAndStructuredEndpointDetails,
            cancellationToken);

        return endpointDetails;
    }

    public async Task<string> GetSpotifyDocumentationReferenceAsync(CancellationToken cancellationToken)
    {
        var path = Path.Combine(FolderName, "spotify-documentation-reference.md");
        var isAlreadyScrapped = File.Exists(path);

        if (isAlreadyScrapped)
            return await File.ReadAllTextAsync(path, cancellationToken);

        var spotifyDocumentationReference =
            await scrappingService.GetMarkdownAsync(DocumentationMainPage, cancellationToken);

        var extractedAndStructuredReference = await spotifyDocumentationReferenceSelectionAssistant.SelectPartAsync(
            spotifyDocumentationReference,
            cancellationToken
        );

        await File.WriteAllTextAsync(path,
            extractedAndStructuredReference,
            cancellationToken
        );

        return spotifyDocumentationReference;
    }
}