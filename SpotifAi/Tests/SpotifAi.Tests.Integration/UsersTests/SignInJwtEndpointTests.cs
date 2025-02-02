using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SpotifAi.Users.Endpoints;

namespace SpotifAi.Tests.Integration.UsersTests;

public class SignInJwtEndpointTests(TestWebApplication app) : IClassFixture<TestWebApplication>
{
    [Fact]
    public async Task SignInJwt_ShouldSucceed_WhenUserExists()
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

        // Act
        var response = await client.PostAsJsonAsync("users/sign-in-jwt", signInUserRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var token = await response.Content.ReadFromJsonAsync<SignInJwtEndpoint.Response>();

        token!.Token.Should().NotBeNullOrEmpty();

        var getMeResponse = await client.GetAsync("users/me");

        getMeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}