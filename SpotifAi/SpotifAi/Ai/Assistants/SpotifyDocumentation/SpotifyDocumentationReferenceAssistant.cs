namespace SpotifAi.Ai.Assistants.SpotifyDocumentation;

internal sealed class SpotifyDocumentationPartSelectionAssistant(IAi ai)
{
    private const string SystemPrompt = @"
        You are a helpful assistant who can extracy information from markdown files and return it in a structured way.
    
        <prompt_objective>
             You are going to be given a markdown file that contains information about the Spotify API.
             Your task is to find references part, extract all sections with descriptions and urls and return them in a structured way.
             Return only the structured result, without any additional text.
        </prompt_objective>

        <example>
            <input>
            # Spotify API Reference
            # abcd
            - [Get Current User's Profile](/documentation/web-api/reference/get-current-users-profile)
            fddasdqwd
            - [Get a List of Current User's Playlists](/documentation/web-api/reference/get-a-list-of-current-users-playlists)
            </input>

            <output>
                - [Get Current User's Profile](/documentation/web-api/reference/get-current-users-profile)
                - [Get a List of Current User's Playlists](/documentation/web-api/reference/get-a-list-of-current-users-playlists)
            </output>
        </example>
        
    ";

    public async Task<string> SelectPartAsync(string markdown, CancellationToken cancellationToken)
    {
        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, SystemPrompt),
                new Message(MessageRole.User, markdown)
            ],
            new AiCompletionSettings
            {
                Model = AiModel.Gpt4OMini,
                JsonMode = false
            },
            cancellationToken
        );

        return aiResponse;
    }
}