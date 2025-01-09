using Microsoft.Extensions.Options;

namespace SpotifAi.Scrapping.Firecrawl;

public sealed record FirecrawlScrapeResponse
{
    public string Markdown { get; init; }
}

public sealed record FirecrawlScrapeResponseResult
{
    public bool Success { get; init; }
    public FirecrawlScrapeResponse Data { get; init; }
}

internal sealed class FirecrawlService(
    HttpClient httpClient,
    IOptions<FirecrawlConfiguration> firecrawlConfiguration
) : IScrappingService
{
    public async Task<string> GetMarkdownAsync(string url, CancellationToken cancellationToken)
    {
        var body = new
        {
            url,
            formats = new List<string> { "markdown" }
        };

        var result =
            await httpClient.PostAsJsonAsync("v1/scrape", body, cancellationToken);

        if (!result.IsSuccessStatusCode) throw new Exception("Failed to scrape the URL");

        var response = await result.Content.ReadFromJsonAsync<FirecrawlScrapeResponseResult>(cancellationToken);

        return response!.Data.Markdown;
    }
}