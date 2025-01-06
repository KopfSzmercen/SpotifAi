using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SpotifAi.Users.Endpoints;

namespace SpotifAi.Tests.Integration.UsersTests;

public class GetMeEndpointTests(TestWebApplication app) : IClassFixture<TestWebApplication>
{
    [Fact]
    public async Task GetMe_WithValidAccessToken_ReturnsUser()
    {
        // Arrange
        var client = app.CreateClient();

        var registerUserRequest = new RegisterUserEndpoint.Request
        {
            Email = "test@t.pl",
            Password = "password"
        };

        await client.PostAsJsonAsync("users/register", registerUserRequest);

        var signInUserRequest = new SignInUserEndpoint.Request
        {
            Email = registerUserRequest.Email,
            Password = registerUserRequest.Password
        };

        await client.PostAsJsonAsync("users/sign-in", signInUserRequest);

        // Act
        var response = await client.GetAsync("users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deserializedResponseContent = await response.Content.ReadFromJsonAsync<GetMeEndpoint.Response>();
        deserializedResponseContent.Should().NotBeNull();
    }
}