using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpotifAi.Spotify;
using SpotifAi.Spotify.AuthorizationStateManager;

namespace SpotifAi.Tests.Integration.SpotifyAuthorizationTests;

public sealed class ReceiveAuthorizationConsentEndpointTests(TestWebApplication app) : IClassFixture<TestWebApplication>
{
    [Fact]
    public async Task ReceiveAuthorizationConsent_WithValidStateAndCode_ReturnsRedirectToRedirectUrl()
    {
        // Arrange
        var opts = new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        };

        var client = app.CreateClient(opts);

        var spotifyConfiguration = app.Services.GetRequiredService<IOptions<SpotifyConfiguration>>();
        var authorizationStateManager = app.Services.GetRequiredService<IAuthorizationStateManager>();

        var state = Guid.NewGuid().ToString();
        await authorizationStateManager.StoreStateValueAsync(state, CancellationToken.None);

        var code = Guid.NewGuid().ToString();

        app.SpotifyApi.SetupGetAccessTokenResponse(
            "code",
            spotifyConfiguration.Value.RedirectUrl,
            "authorization_code",
            spotifyConfiguration.Value.ClientId,
            spotifyConfiguration.Value.ClientSecret
        );

        // Act
        var response = await client.GetAsync($"spotify/receive-authorization-consent?state={state}&code={code}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Be(spotifyConfiguration.Value.RedirectUrl);

        var stateIsCorrectlyInvalidated =
            await authorizationStateManager.ValidateStateValueAsync(state, CancellationToken.None);

        stateIsCorrectlyInvalidated.Should().BeFalse();
    }
}