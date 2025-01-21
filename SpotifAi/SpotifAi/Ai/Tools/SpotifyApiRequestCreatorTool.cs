using System.Text.Json;
using SpotifAi.Ai.Agent;
using SpotifAi.Ai.Assistants.SpotifyDocumentation;

namespace SpotifAi.Ai.Tools;

internal sealed class SpotifyApiRequestCreatorTool(SpotifyRequestCreatorAssistant requestCreatorAssistant) : AgentTool(
    Tool.SpotifyApiRequestCreator,
    """
    This tool can create a request for a specific Spotify API endpoint. It uses the endpoint path and parameters to generate a request object. 
    Parameters: endpoint specification, metadata, and task
    Result: JSON object with the request method, url, and body
    """
)
{
    public override async Task<string> ExecuteAsync(string parameters, CancellationToken cancellationToken)
    {
        var assistantResult =
            await requestCreatorAssistant.CreateRequestAsync(
                parameters,
                "",
                parameters,
                cancellationToken
            );

        return JsonSerializer.Serialize(assistantResult);
    }
}