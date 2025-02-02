using SpotifAi.Ai.Assistants.SpotifyDocumentation;

namespace SpotifAi.Ai.Assistants;

internal static class AssistantsModule
{
    public static IServiceCollection AddAssistants(this IServiceCollection services)
    {
        services.AddSingleton<SpotifyDocumentationReferenceSelectionAssistant>();

        return services;
    }
}