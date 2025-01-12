using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using SpotifAi.Auth.RequestContext;
using SpotifAi.Persistence;
using SpotifAi.Spotify.Api.Authorization;
using SpotifAi.Utils;

namespace SpotifAi.Spotify.Api;

internal sealed class AttachUserAccessTokenDelegatingHandler(
    AppDbContext dbContext,
    IRequestContext requestContext,
    IClock clock,
    ISpotifyAuthorizationApi spotifyAuthorizationApi
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var spotifyToken = await dbContext.SpotifyAccessTokens
            .FirstOrDefaultAsync(x => x.UserId == requestContext.Id, cancellationToken);

        if (spotifyToken is null)
            throw new InvalidOperationException("User has not authorized Spotify");

        if (spotifyToken.ExpiresAt < clock.Now)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", spotifyToken.AccessToken);

        var refreshToken = await spotifyAuthorizationApi.RefreshAccessTokenAsync(spotifyToken.RefreshToken);

        spotifyToken.AccessToken = refreshToken.AccessToken;
        spotifyToken.ExpiresAt = clock.Now.AddSeconds(refreshToken.ExpiresIn);
        spotifyToken.RefreshToken = refreshToken.RefreshToken;

        await dbContext.SaveChangesAsync(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken.AccessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}