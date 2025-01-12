using System.Text.Json;

namespace SpotifAi.Ai.Assistants.SpotifyDocumentation;

internal sealed class SpotifyDocumentationPartSelectionAssistant(IAi ai)
{
    private const string SystemPrompt = @"
          You are a helpful assistant who can decide which endpoint of the Spotify API is the most relevant for a given task.

          <prompt_objective>
               You are going to be given a list of endpoints with short descriptions and a task description.
               Your task is to select the most relevant endpoint for the given task.
               Return only the selected endpoint, without any additional text.
          </prompt_objective>

          <prompt_rules>
               - Always return the most probable endpoint if you are not 100% sure about the answer.
               - Always return only the endpoint path eg. /documentation/web-api/reference/get-current-users-profile
               - Endpoint must be in the given list of endpoints, do not invent new endpoints.
               - The result must not have any additional characters, only the endpoint path.
               - The result must be in a required JSON format: { ""result"": ""/documentation/web-api/reference/get-current-users-profile"" }
          </prompt_rules>

          <example>
              <input>
               <task>You need to get the current user's profile.</task>
               <documentation_reference>
                    - [Get Current User's Profile](/documentation/web-api/reference/get-current-users-profile)
                    - [Get a List of Current User's Playlists](/documentation/web-api/reference/get-a-list-of-current-users-playlists)
                    - [Get Artist's Albums](/documentation/web-api/reference/get-an-artists-albums)
               </documentation_reference>
              </input>

              <output>
                  /documentation/web-api/reference/get-current-users-profile
              </output>
         </example>
     ";

    public async Task<string> SelectPartAsync(
        string documentationReference,
        string task,
        CancellationToken cancellationToken
    )
    {
        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, SystemPrompt),
                new Message(MessageRole.User,
                    $@"
                    <task>{task}</task>
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