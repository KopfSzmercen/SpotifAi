using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SpotifAi.Users.Endpoints;

namespace SpotifAi.Tests.Integration.UsersTests;

public class SignInUserEndpointTests(TestWebApplication app) : IClassFixture<TestWebApplication>
{
    [Fact]
    public async Task SignInUser_ShouldSucceed_WhenUserExists()
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
        var response = await client.PostAsJsonAsync("users/sign-in", signInUserRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}