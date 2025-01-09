namespace SpotifAi.Scrapping;

public interface IScrappingService
{
    Task<string> GetMarkdownAsync(string url, CancellationToken cancellationToken);
}