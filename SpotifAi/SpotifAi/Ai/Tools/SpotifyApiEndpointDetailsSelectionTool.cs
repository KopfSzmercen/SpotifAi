using SpotifAi.Ai.Agent;
using SpotifAi.Ai.Assistants.SpotifyDocumentation;
using SpotifAi.Scrapping;

namespace SpotifAi.Ai.Tools;

internal sealed class SpotifyApiEndpointDetailsSelectionTool(
    IAi ai,
    SpotifyDocumentationScrapping spotifyDocumentationScrapping,
    SpotifyEndpointParametersSelectionAssistant parametersSelectionAssistant
) : AgentTool(Tool.SpotifyApiEndpointDetailsSelection,
    """
    This tool has access to details a specific Spotify API endpoint. It analyzes what parameters are available for the endpoint, such as query parameters, path parameters, and request body parameters.
    Parameters: url for the documentation e.g: /documentation/web-api/reference/get-current-users-profile
    The tool will return the extracted parameters in a structured format.
    """
)
{
    public override async Task<string> ExecuteAsync(string parameters, CancellationToken cancellationToken)
    {
        var preparedParameters = await PrepareParametersAsync(parameters, cancellationToken);

        var selectedDocumentationPartDetails = await spotifyDocumentationScrapping.GetSpotifyEndpointDetailsAsync(
            preparedParameters,
            cancellationToken
        );

        var endpointWithParameters = await parametersSelectionAssistant.SelectPartAsync(
            selectedDocumentationPartDetails,
            cancellationToken
        );

        return endpointWithParameters;
    }

    private async Task<string> PrepareParametersAsync(string parameters, CancellationToken cancellationToken)
    {
        const string systemPrompt = @"You are an assistant who can extract url path from a text.
                Find a url path in a given text and return it without any additional text and characters.
                Make sure endpoint does not start with the domain name or '/' 
                Example response: documentation/web-api/reference/get-current-users-profile
            ";

        return await ai.GetCompletionAsync(
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
    }
}