using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpotifAi.Spotify;
using SpotifAi.Spotify.AuthorizationStateManager;

namespace SpotifAi.Tests.Integration.SpotifyAuthorizationTests;

public class InitializeSpotifyAuthorizationEndpointTests(TestWebApplication app)
    : IClassFixture<TestWebApplication>
{
    [Fact]
    public async Task GetAuthorizationUrl_ReturnsRedirectToSpotifyAuthorizationUrl()
    {
        // Arrange
        var opts = new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        };

        var client = app.CreateClient(opts);

        var spotifyConfiguration = app.Services.GetRequiredService<IOptions<SpotifyConfiguration>>();
        var authorizationStateManager = app.Services.GetRequiredService<IAuthorizationStateManager>();

        // Act
        var response = await client.PostAsync("spotify/initialize-authorization", new StringContent(""));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().StartWith(spotifyConfiguration.Value.SpotifyAuthorizationUrl);

        var state = response.Headers.Location!.Query.Split("state=")[1];

        var stateIsCorrectlyAssignedToRequestAndStored =
            await authorizationStateManager.ValidateStateValueAsync(state, CancellationToken.None);

        stateIsCorrectlyAssignedToRequestAndStored.Should().BeTrue();
    }
}