using System.Text;
using Microsoft.Extensions.Options;

namespace SpotifAi.Spotify.Api.Authorization;

internal sealed class ApplyAuthorizationApiDefaultHeadersDelegatingHandler(
    IOptions<SpotifyConfiguration> spotifyConfiguration)
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string, string>
        {
            { "content-type", "application/x-www-form-urlencoded" },
            {
                "Authorization",
                $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{spotifyConfiguration.Value.ClientId}:{spotifyConfiguration.Value.ClientSecret}"))}"
            }
        };

        foreach (var (key, value) in headers) request.Headers.TryAddWithoutValidation(key, value);

        return base.SendAsync(request, cancellationToken);
    }
}