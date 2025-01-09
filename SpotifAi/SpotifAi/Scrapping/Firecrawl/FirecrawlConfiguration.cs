namespace SpotifAi.Scrapping.Firecrawl;

public sealed class FirecrawlConfiguration
{
    public const string SectionName = "Firecrawl";
    public string BaseUrl { get; set; }

    public string ApiKey { get; set; }
}