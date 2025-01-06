using SpotifAi.Users.Endpoints;

namespace SpotifAi.Users;

internal static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }

    public static IApplicationBuilder UseUsersModule(this WebApplication app)
    {
        var group = app
            .MapGroup("users")
            .WithTags("Users");

        group
            .RegisterRegisterUserEndpoint()
            .RegisterSignInUserEndpoint()
            .RegisterGetMeEndpoint();

        return app;
    }
}