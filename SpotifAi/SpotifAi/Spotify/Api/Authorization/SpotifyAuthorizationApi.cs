using Microsoft.Extensions.Options;

namespace SpotifAi.Spotify.Api.Authorization;

internal sealed class SpotifyAuthorizationApi(
    HttpClient httpClient,
    IOptions<SpotifyConfiguration> spotifyConfiguration
) : ISpotifyAuthorizationApi
{
    public async Task<RequestAccessTokenResponse> GetAccessTokenAsync(string code)
    {
        var form = new Dictionary<string, string>
        {
            { "code", code },
            { "redirect_uri", spotifyConfiguration.Value.RedirectUrl },
            { "grant_type", "authorization_code" }
        };

        var response =
            await httpClient.PostAsync(spotifyConfiguration.Value.SpotifyTokenUrl, new FormUrlEncodedContent(form));

        if (!response.IsSuccessStatusCode) throw new Exception("Failed to get access token.");

        var deserializedResponse = await response.Content.ReadFromJsonAsync<RequestAccessTokenResponse>();

        return deserializedResponse!;
    }

    public async Task<RequestAccessTokenResponse> RefreshAccessTokenAsync(string refreshToken)
    {
        var form = new Dictionary<string, string>
        {
            { "refresh_token", refreshToken },
            { "grant_type", "refresh_token" }
        };

        var response =
            await httpClient.PostAsync(spotifyConfiguration.Value.SpotifyTokenUrl, new FormUrlEncodedContent(form));

        if (!response.IsSuccessStatusCode) throw new Exception("Failed to refresh access token.");

        var deserializedResponse = await response.Content.ReadFromJsonAsync<RequestAccessTokenResponse>();

        return deserializedResponse!;
    }
}