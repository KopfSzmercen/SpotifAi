using SpotifAi.Ai.Tools;
using HttpMethod = SpotifAi.Ai.Tools.HttpMethod;

namespace SpotifAi.Spotify.Api;

internal sealed class SendSpotifyRequest(HttpClient httpClient)
{
    public async Task<string?> SendRequestAsync(SpotifyRequest request, CancellationToken cancellationToken)
    {
        switch (request.Method)
        {
            case HttpMethod.Get:
            {
                var response = await httpClient.GetAsync(request.Url, cancellationToken);
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            case HttpMethod.Post:
            {
                var response =
                    await httpClient.PostAsync(request.Url, new StringContent(request.Body!), cancellationToken);
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            case HttpMethod.Put:
            {
                var response =
                    await httpClient.PutAsync(request.Url, new StringContent(request.Body!), cancellationToken);
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }

            case HttpMethod.Delete:
            default:
                return "No matching api method found.";
        }
    }
}