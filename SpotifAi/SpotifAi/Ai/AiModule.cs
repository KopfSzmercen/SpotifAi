using SpotifAi.Ai.Agent;
using SpotifAi.Ai.Assistants;
using SpotifAi.Ai.Tools;

namespace SpotifAi.Ai;

internal static class AiModule
{
    public static IServiceCollection AddAiModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenAiConfiguration>(configuration.GetSection(OpenAiConfiguration.SectionName));

        services.AddSingleton<IAi, OpenAiService>();

        services.AddAssistants();

        services.AddAgentTools();

        services.AddScoped<AgentState>(sp =>
        {
            var allTools = sp.GetServices<AgentTool>();
            return new AgentState(allTools);
        });

        services.AddScoped<Agent.Agent>();

        return services;
    }
}