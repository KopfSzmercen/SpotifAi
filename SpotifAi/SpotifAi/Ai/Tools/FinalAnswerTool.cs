using SpotifAi.Ai.Agent;

namespace SpotifAi.Ai.Tools;

public class FinalAnswerTool(IAi ai) : AgentTool(
    Tool.FinalAnswer,
    """
        This tool is used to parse the final answer to a user friendly structure in Markdown format based on the initial query and the conversation context, specifically the final result or failure.
        Parameters: Takes the initial user query and the final result or failure. If needed, it can also take the conversation context and steps taken to reach the final result.
        Result: The final answer in a user-friendly format, preferably in a well-structured markdown.
    """
)
{
    public override async Task<string> ExecuteAsync(string parameters, CancellationToken cancellationToken)
    {
        const string systemPrompt = @"You are an assistant who can format the final answer in user-friendly format.
              Format the final answer in a user-friendly format, based on the initial user query and the information gathered during the conversation.
              If possible, the result should be a well-structured markdown which can be easily read and understood by the user.
        ";


        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, systemPrompt),
                new Message(MessageRole.User, $"""
                                               
                                                       <conversation>
                                                           {parameters}
                                                       </conversation>

                                               """)
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