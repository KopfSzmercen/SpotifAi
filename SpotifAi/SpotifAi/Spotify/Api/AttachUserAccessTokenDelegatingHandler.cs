using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using SpotifAi.Auth.RequestContext;
using SpotifAi.Persistence;

namespace SpotifAi.Spotify.Api;

internal sealed class AttachUserAccessTokenDelegatingHandler(
    AppDbContext dbContext,
    IRequestContext requestContext
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var spotifyToken = await dbContext.SpotifyAccessTokens
            .FirstOrDefaultAsync(x => x.UserId == requestContext.Id, cancellationToken);

        if (spotifyToken is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", spotifyToken.AccessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}