namespace SpotifAi.Users;

internal sealed class SpotifyAccessToken
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public User User { get; set; }

    public Guid UserId { get; set; }
}