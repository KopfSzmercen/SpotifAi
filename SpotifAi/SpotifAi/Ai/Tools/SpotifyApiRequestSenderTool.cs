using System.Text.Json;
using SpotifAi.Ai.Agent;
using SpotifAi.Ai.Assistants.SpotifyDocumentation;
using SpotifAi.Spotify.Api;

namespace SpotifAi.Ai.Tools;

internal sealed class SpotifyApiRequestSenderTool(
    IAi ai,
    SendSpotifyRequest sendSpotifyRequest
) : AgentTool(Tool.SpotifyApiRequestSender,
    """
    This tool can send a request to the Spotify API using the provided request object. It sends the request to the Spotify API and returns the response object. 
    Parameters: JSON object with the request method, url, and body
    Result: JSON object with the response from the Spotify API
    """
)
{
    public override async Task<string> ExecuteAsync(string parameters, CancellationToken cancellationToken)
    {
        var preparedParameters = await PrepareParametersAsync(parameters, cancellationToken);

        var response = await sendSpotifyRequest.SendRequestAsync(preparedParameters, cancellationToken);

        return response ?? "Action returned no response";
    }

    private async Task<SpotifyRequest> PrepareParametersAsync(string parameters, CancellationToken cancellationToken)
    {
        const string systemPrompt = @"You are an assistant who can extract JSON from a text.
                Find a JSON object in a given text and return it without any additional text and characters.
                Example response {""Method"": ""Post"", ""Url"": ""https://api.spotify.com/v1/me/player"", ""Body"": ""{\""device_ids\"":[""74ASZWbe4lXaubB36ztrGX\""],\""play\"":true}""}
             ";

        var preparedParametersJson = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, systemPrompt),
                new Message(MessageRole.User, parameters)
            ],
            new AiCompletionSettings
            {
                Model = AiModel.Gpt4OMini,
                JsonMode = false
            },
            cancellationToken
        );

        return JsonSerializer.Deserialize<SpotifyRequest>(preparedParametersJson) ??
               throw new Exception("Failed to deserialize SpotifyRequest");
    }
}