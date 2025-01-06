using System.Text.Json.Serialization;

namespace SpotifAi.Spotify.Api.Authorization;

public sealed record RequestAccessTokenResponse
{
    //<summary>
    // 	An access token that can be provided in subsequent calls, for example to Spotify Web API services.
    // </summary>
    [JsonPropertyName("access_token")] public string AccessToken { get; init; } = null!;

    //<summary>
    // How the access token may be used: always "Bearer".
    // </summary>
    [JsonPropertyName("token_type")] public string TokenType { get; init; } = null!;

    //<summary>
    // The time period (in seconds) for which the access token is valid.
    // </summary>
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; init; }

    //<summary>
    // A token that can be sent to the Spotify Accounts service in place of an authorization code.
    //</summary>
    [JsonPropertyName("refresh_token")] public string RefreshToken { get; init; } = null!;

    //<summary>
    // A space-separated list of scopes that have been granted for this access token.
    //</summary>
    [JsonPropertyName("scope")] public string Scope { get; init; } = null!;
}