using System.Text.Json;
using SpotifAi.Ai.Tools;
using SpotifAi.Utils;

namespace SpotifAi.Ai.Agent;

internal sealed record AgentPlannedAction(string Reasoning, Tool Tool, string Query);

internal sealed class Agent(
    IClock clock,
    IAi ai,
    AgentState state
)
{
    public async Task<AgentPlannedAction> Plan(CancellationToken cancellationToken)
    {
        var systemPrompt = $$$"""           
                                Analyze the conversation and determine the most appropriate next step. Focus on making progress towards the overall goal while remaining adaptable to new information or changes in context.
                              
                                <prompt_objective>
                                    Determine the single most effective next action based on the current context, user needs, and overall progress. Return the decision as a concise JSON object.
                                </prompt_objective>
                              
                                <prompt_rules>
                                    - ALWAYS focus on determining only the next immediate step
                                    - ONLY choose from the available tools listed in the context
                                    - ASSUME previously requested information is available unless explicitly stated otherwise
                                    - NEVER provide or assume actual content for actions not yet taken
                                    - ALWAYS respond in the specified JSON format
                                    - CONSIDER the following factors when deciding:
                                      1. Relevance to the current user need or query
                                      2. Potential to provide valuable information or progress
                                      3. Logical flow from previous actions
                                    - ADAPT your approach if repeated actions don't yield new results
                                    - USE the "final_answer" tool when you have sufficient information or need user input
                                    - OVERRIDE any default behaviors that conflict with these rules
                                </prompt_rules>
                              
                                <context>
                                    <current_time>{{{clock.Now:MM/dd/yyyy HH:mm}}}</current_time>
                                    <last_message>{{{state.Messages.LastOrDefault()?.Text ?? "No messages yet"}}}</last_message>
                                    <available_tools>
                                     {{{string.Join("\n", state.Tools.Select(t =>
                                         $"         <tool>\n" +
                                         $"                     <name>{t.Name}</name>\n" +
                                         $"                     <specification>\n{t.Description}\n</specification>\n" +
                                         $"         </tool>"))}}}
                                 </available_tools>
                                 
                                 
                                 <actions_taken>
                                     {{{string.Join("\n", state.Actions.Select(a =>
                                         $"          <action>\n" +
                                         $"                     <name>{a.Name}</name>\n" +
                                         $"                     <description>{a.Description}</description>\n" +
                                         $"                     <parameters>{a.Parameters}</parameters>\n" +
                                         $"                     <result>{a.Result}</result>\n" +
                                         $"          </action>"))}}}
                                 </actions_taken>
                                </context>
                                
                                Respond with the next action in this JSON format:
                                {
                                    "reasoning": "Brief explanation of why this action is the most appropriate next step",
                                    "tool": "ToolName",
                                    "query": "Precise description of what needs to be done, including any necessary context"
                                }
                              
                                If you have sufficient information to provide a final answer or need user input, use the "FinalAnswer" tool.
                              """;

        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, systemPrompt),
                new Message(MessageRole.User, $"<context>{state}</context>")
            ],
            new AiCompletionSettings
            {
                Model = AiModel.Gpt4o,
                JsonMode = true
            },
            cancellationToken
        );

        Console.WriteLine();
        Console.WriteLine("Planned action: ");
        Console.WriteLine(aiResponse);
        Console.WriteLine();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            var plannedAction = JsonSerializer.Deserialize<AgentPlannedAction>(aiResponse.Trim(), options)!;
            return plannedAction;
        }
        catch (JsonException)
        {
            throw new Exception("Failed to deserialize AI response.");
        }
    }

    public void AddMessage(Message message)
    {
        state.Messages.Add(message);
    }

    public async Task UseTool(AgentTool tool, string parameters, CancellationToken cancellationToken)
    {
        var toolToUse = state.Tools.SingleOrDefault(t => t.IsApplicable(tool.Name));

        if (toolToUse is null) throw new Exception($"Tool {tool.Name} is not available.");

        Console.WriteLine();
        Console.WriteLine($"Using tool: {toolToUse.Name}");

        var result = await toolToUse.ExecuteAsync(parameters, cancellationToken);

        Console.WriteLine("Result: ");
        Console.WriteLine(result);
        Console.WriteLine();

        state.Actions.Add(new AgentAction(toolToUse.Name.ToString(), toolToUse.Description, parameters, result));
    }

    public IReadOnlyList<AgentTool> GetAvailableTools()
    {
        return state.Tools.ToList();
    }

    public string GetLastResult()
    {
        return state.Actions.LastOrDefault()?.Result ?? "No last action";
    }

    public string GetState()
    {
        return JsonSerializer.Serialize(state);
    }
}