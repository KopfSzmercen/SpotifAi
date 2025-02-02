using SpotifAi.Ai.Agent;

namespace SpotifAi.Ai.Tools;

public class FinalAnswerTool(IAi ai) : AgentTool(
    Tool.FinalAnswer,
    """
        This tool is used to parse the final answer to a user friendly structure in Markdown format based on the initial query and the conversation context, specifically the final result or failure.
        Parameters: Takes the initial user query and the final result or failure. If needed, it can also take the conversation context and steps taken to reach the final result.
        Result: The final answer in a user-friendly format.
    """
)
{
    public override async Task<string> ExecuteAsync(string parameters, CancellationToken cancellationToken)
    {
        const string systemPrompt = @"You are an assistant who can format the final answer in user-friendly format.
              Format the final answer in a user-friendly format, based on the initial user query and the information gathered during the conversation.
              The result should be a nice, responsive html snippet in a modern style. Focus on readability and user experience. 
              The content will be then rendered in a web browser.
              Rules:
                - the content should not contain base html tags like <html>, <head>, <body>, etc. since it will be injected into an existing html page
                - the result must not have any external dependencies like css or js files
                - the result must not contain any special characters like quotes, backticks, new lines
                - the result must be a single string with the html content
                - use only black color and do not add background color
                - try to escape characters lik ' eg. user's -> user\'s
                - the result should be the same like result of a stored rich text field so it can be rendered in a web browser using innerHTML property
                - try to sound like you are a human assistant, not a robot. Describe yourself, not the tools
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