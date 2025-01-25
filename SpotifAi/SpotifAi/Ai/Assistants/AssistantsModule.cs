using SpotifAi.Ai.Assistants.SpotifyDocumentation;

namespace SpotifAi.Ai.Assistants;

internal static class AssistantsModule
{
    public static IServiceCollection AddAssistants(this IServiceCollection services)
    {
        services.AddSingleton<SpotifyDocumentationReferenceSelectionAssistant>();

        services.AddSingleton<SpotifyEndpointParametersSelectionAssistant>();

        services.AddSingleton<SpotifyRequestCreatorAssistant>();

        services.AddSingleton<SpotifyResponsePrettifierAssistant>();

        return services;
    }
}