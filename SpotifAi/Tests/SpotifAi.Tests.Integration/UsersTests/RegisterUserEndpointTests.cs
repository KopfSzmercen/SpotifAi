using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpotifAi.Persistence;
using SpotifAi.Users.Endpoints;

namespace SpotifAi.Tests.Integration.UsersTests;

public class RegisterUserEndpointTests(TestWebApplication app) : IClassFixture<TestWebApplication>
{
    [Fact]
    public async Task RegisterUser_ShouldSucceed_WhenEmailIsUnique()
    {
        // Arrange
        var client = app.CreateClient();

        var registerUserRequest = new RegisterUserEndpoint.Request
        {
            Email = "test@t.pl",
            Password = "password"
        };

        // Act
        var response = await client.PostAsJsonAsync("users/register", registerUserRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == registerUserRequest.Email);

        user.Should().NotBeNull();
    }
}