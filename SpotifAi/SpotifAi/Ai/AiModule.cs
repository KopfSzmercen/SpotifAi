using SpotifAi.Ai.Assistants.SpotifyDocumentation;

namespace SpotifAi.Ai;

internal static class AiModule
{
    public static IServiceCollection AddAiModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenAiConfiguration>(configuration.GetSection(OpenAiConfiguration.SectionName));

        services.AddSingleton<IAi, OpenAiService>();

        services.AddSingleton<SpotifyDocumentationPartSelectionAssistant>();

        services.AddSingleton<SpotifyDocumentationReferenceSelectionAssistant>();

        services.AddSingleton<SpotifyEndpointParametersSelectionAssistant>();

        services.AddSingleton<SpotifyRequestCreatorAssistant>();

        services.AddSingleton<SpotifyResponsePrettifierAssistant>();

        return services;
    }
}