using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpotifAi.Spotify;
using SpotifAi.Spotify.Api.Authorization;

namespace SpotifAi.Tests.Integration.SpotifyAuthorizationTests;

public class RefreshTokenTests(TestWebApplication app) : IClassFixture<TestWebApplication>
{
    [Fact]
    public async Task RefreshToken_WithValidRefreshToken_ReturnsNewAccessToken()
    {
        // Arrange
        app.CreateClient();

        var spotifyConfiguration = app.Services.GetRequiredService<IOptions<SpotifyConfiguration>>();

        var spotifyAuthorizationApi = app.Services.GetRequiredService<ISpotifyAuthorizationApi>();

        var refreshToken = Guid.NewGuid().ToString();

        app.SpotifyApi.SetupRefreshAccessTokenResponse(
            refreshToken,
            spotifyConfiguration.Value.ClientId,
            spotifyConfiguration.Value.ClientSecret
        );

        // Act
        var response = await spotifyAuthorizationApi.RefreshAccessTokenAsync(refreshToken);

        // Assert
        response.AccessToken.Should().NotBeNullOrEmpty();
        response.TokenType.Should().Be("Bearer");
    }
}