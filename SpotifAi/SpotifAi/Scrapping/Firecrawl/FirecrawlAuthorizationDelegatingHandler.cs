using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace SpotifAi.Scrapping.Firecrawl;

internal sealed class FirecrawlAuthorizationDelegatingHandler(IOptions<FirecrawlConfiguration> firecrawlConfiguration)
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", firecrawlConfiguration.Value.ApiKey);

        return base.SendAsync(request, cancellationToken);
    }
}