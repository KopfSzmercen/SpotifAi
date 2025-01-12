namespace SpotifAi.Ai.Assistants.SpotifyDocumentation;

internal sealed class SpotifyResponsePrettifierAssistant(IAi ai)
{
    private const string SystemPrompt = @"
               You are a helpful assistant who can prettify the response from Spotify API according to the given task so it best fits the user's needs.
               
               <prompt_objective>
                    You are going to be given a response from Spotify API and a task.
                    You have to prettify the response to make it more readable and understandable.
                    Response should be in a user-friendly format like chat message according to the given task.
               </prompt_objective>
               
               <prompt_example>
                    <response>
                         {
                         ""artist_playing"":[""Bruce Springsteen""],
                         ""song"":""Born in the U.S.A."",
                         }
                    </response>
                    <task>What's the song I'm currently playing?</task>
                    <output>
                         You are currently playing a song Born in the U.S.A. by Bruce Springsteen.
                    </output>
               </prompt_example>
          ";

    public async Task<string> PrettifyResponseAsync(string response, string task, CancellationToken cancellationToken)
    {
        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, SystemPrompt),
                new Message(MessageRole.User,
                    $@"
                      <response>
                      {response}
                      </response>
                      <task>{task}</task>
                     "
                )
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