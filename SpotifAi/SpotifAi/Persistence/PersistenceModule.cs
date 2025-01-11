using Microsoft.EntityFrameworkCore;

namespace SpotifAi.Persistence;

internal static class PersistenceModule
{
    public static IServiceCollection AddPersistenceModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Database"));
        });

        return services;
    }

    public static async Task<IApplicationBuilder> UsePersistenceModuleAsync(this WebApplication app)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        await ApplyMigrations(app);
        return app;
    }

    private static async Task ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}