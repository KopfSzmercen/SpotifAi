using SpotifAi.Spotify;

namespace SpotifAi.Tests.Integration;

internal static class DefaultTestSpotifyConfiguration
{
    public static SpotifyConfiguration SpotifyConfiguration => new()
    {
        ClientId = "test-client-id",
        Scope =
            "ugc-image-upload user-read-playback-state user-modify-playback-state user-read-currently-playing app-remote-control streaming user-read-email user-read-private user-library-read user-library-modify user-read-playback-position user-read-recently-played user-top-read user-read-playback-position user-read-recently-played user-follow-read user-follow-modify playlist-read-collaborative playlist-modify-public playlist-read-private playlist-modify-private user-read-currently-playing user-modify-playback-state user-read-playback-state user-read-private user-read-email user-library-read user-library-modify user-read-playback-position user-read-recently-played user-top-read user-follow-read user-follow-modify playlist-read-collaborative playlist-modify-public playlist-read-private playlist-modify-private user-read-currently-playing user-modify-playback-state user-read-playback-state user-read-private user-read-email user-library-read user-library-modify user-read-playback-position user-read-recently-played user-top-read user-follow-read user-follow-modify playlist-read-collaborative playlist-modify-public playlist-read-private playlist-modify-private user-read-currently-playing user-modify-playback-state user-read-playback-state user-read-private user-read-email user-library-read user-library-modify user-read-playback-position user-read-recently-played user-top-read user-follow-read user-follow-modify playlist-read-collaborative playlist-modify-public playlist-read-private playlist-modify-private user-read-currently-playing user-modify-playback-state user-read-playback-state user-read-private user-read-email user-library-read user-library-modify user-read-playback-position user-read-recently-played user-top-read user-follow-read user-follow-modify playlist-read-collaborative playlist-modify-public playlist-read-private playlist-modify-private user-read-currently-playing user-modify-playback-state user-read-playback-state user-read-private user-read-email user-library-read user-library-modify user-read-playback-position user-read-recently-played user-top-read user-follow-read user-follow-modify playlist-read-collaborative playlist-modify-public playlist-read-private playlist-modify-private user-read-currently-playing user-modify-playback-state user-read-playback-state user-read-private user-read-email user-library-read user-library-modify user-read-playback-position user-read-recently-played user-top-read user-follow-read user-follow-modify playlist-read-collaborative playlist-modify-public playlist-read-private playlist-modify-private user-read-currently-playing user-mod",
        ClientSecret = "4ab1e141e27c46a1891ccca8623f5c2a",
        RedirectUrl = "http://localhost:5000/spotify/authorization-callback",
        SpotifyAuthorizationUrl = "https://accounts.spotify.com/authorize",
        SpotifyTokenUrl = "https://accounts.spotify.com/api/token"
    };
}