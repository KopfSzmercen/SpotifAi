namespace SpotifAi.Ai.Tools;

internal static class ToolsModule
{
    public static void AddAgentTools(this IServiceCollection services)
    {
        services.AddSingleton<AgentTool, SpotifyApiDocumentationPartSelectionTool>();

        services.AddSingleton<AgentTool, SpotifyApiEndpointDetailsSelectionTool>();

        services.AddSingleton<AgentTool, SpotifyApiRequestCreatorTool>();

        services.AddSingleton<AgentTool, FinalAnswerTool>();
    }
}