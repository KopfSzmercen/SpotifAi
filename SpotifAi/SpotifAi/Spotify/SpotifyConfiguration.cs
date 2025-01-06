namespace SpotifAi.Spotify;

public class SpotifyConfiguration
{
    public const string SectionName = "Spotify";
    public string ClientId { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;

    public string Scope { get; set; } = null!;

    public string SpotifyAuthorizationUrl { get; set; } = null!;

    public string SpotifyTokenUrl { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;
}