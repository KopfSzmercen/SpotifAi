using System.Text.Json;
using SpotifAi.Ai.Agent;
using SpotifAi.Scrapping;

namespace SpotifAi.Ai.Tools;

internal sealed class SpotifyApiDocumentationPartSelectionTool(
    SpotifyDocumentationScrapping spotifyDocumentationScrapping,
    IAi ai
) :
    AgentTool(Tool.SpotifyApiDocumentationPartSelection,
        """
        <tool_description>
            This tool has access to a summary of SpotifyApi endpoints with their names and routes.
            This tool selects the best endpoint which can be used to perform a specific task.
        </tool_description>

        <tool_parameters>
            Description of the endpoint to search in the reference e.g: "Endpoint to get the current user's profile"
        </tool_parameters>

        <tool_result>
            Result: result is always the endpoint path, eg. documentation/web-api/reference/get-current-users-profile
        </tool_result>

        <tool_action_example>
            InputParameter: Get the endpoint to get the current user's profile
            OutputResult: documentation/web-api/reference/get-current-users-profile
        </tool_action_example>
        """
    )
{
    private const string SystemPrompt = """
                                        You are a helpful assistant who can decide which endpoint of the Spotify API is the most relevant for a given task.
                                            
                                        <prompt_objective>
                                                You are going to be given a list of endpoints with short descriptions and a task description.
                                                Your task is to select the most relevant endpoint for the given task.
                                                Return only the selected endpoint, without any additional text.
                                        </prompt_objective>
                                            
                                        <prompt_rules>
                                                 - Always return the most probable endpoint if you are not 100% sure about the answer.
                                                 - Always return only the endpoint path eg. documentation/web-api/reference/get-current-users-profile
                                                 - Endpoint must be in the given list of endpoints, do not invent new endpoints.
                                                 - The result must not have any additional characters, only the endpoint path.
                                                 - The result must be in a required JSON format: { "result": "documentation/web-api/reference/get-current-users-profile" }
                                        </prompt_rules>
                                            
                                        <example>
                                               <input>
                                                    <desired_endpoint_description>You need to get the current user's profile.</desired_endpoint_description>
                                                     <documentation_reference>
                                                       - [Get Current User's Profile](/documentation/web-api/reference/get-current-users-profile)
                                                       - [Get a List of Current User's Playlists](/documentation/web-api/reference/get-a-list-of-current-users-playlists)
                                                       - [Get Artist's Albums](/documentation/web-api/reference/get-an-artists-albums)
                                                     </documentation_reference>
                                                </input>
                                            
                                                <output>
                                                      { "result": "documentation/web-api/reference/get-current-users-profile" }
                                                </output>
                                        </example>
                                        """;

    public override async Task<string> ExecuteAsync(string parameters, CancellationToken cancellationToken)
    {
        var documentationReference =
            await spotifyDocumentationScrapping.GetSpotifyDocumentationReferenceAsync(cancellationToken);

        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, SystemPrompt),
                new Message(MessageRole.User,
                    $@"
                    <desired_endpoint_description>{parameters}</desired_endpoint_description>
                    <documentation_reference>
                    {documentationReference}
                    </documentation_reference>
                "
                )
            ],
            new AiCompletionSettings
            {
                Model = AiModel.Gpt4OMini,
                JsonMode = true
            },
            cancellationToken
        );

        var deserializedResponse = JsonSerializer.Deserialize<JsonDocument>(aiResponse);
        var result = deserializedResponse!.RootElement.GetProperty("result").GetString();

        return result!;
    }
}