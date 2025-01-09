using SpotifAi.Ai.Assistants.SpotifyDocumentation;

namespace SpotifAi.Ai;

internal static class AiModule
{
    public static IServiceCollection AddAiModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenAiConfiguration>(configuration.GetSection(OpenAiConfiguration.SectionName));

        services.AddSingleton<IAi, OpenAiService>();

        services.AddSingleton<SpotifyDocumentationPartSelectionAssistant>();

        return services;
    }
}