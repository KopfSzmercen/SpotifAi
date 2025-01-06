namespace SpotifAi.Spotify.Api.Authorization;

public interface ISpotifyAuthorizationApi
{
    Task<RequestAccessTokenResponse> GetAccessTokenAsync(string code);

    Task<RequestAccessTokenResponse> RefreshAccessTokenAsync(string refreshToken);
}