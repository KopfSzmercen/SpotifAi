using System.Text.Json;
using SpotifAi.Ai.Assistants.SpotifyDocumentation;
using SpotifAi.Scrapping;
using SpotifAi.Spotify.Api;
using SpotifAi.Utils;

namespace SpotifAi.Ai.Agent;

internal sealed record AgentPlannedAction(string Reasoning, Tool Tool, string Query);

internal sealed class Agent(
    IClock clock,
    IAi ai,
    SpotifyDocumentationPartSelectionAssistant partSelectionAssistant,
    SpotifyEndpointParametersSelectionAssistant parametersSelectionAssistant,
    SpotifyRequestCreatorAssistant requestCreatorAssistant,
    SpotifyDocumentationScrapping spotifyDocumentationScrapping,
    SendSpotifyRequest sendSpotifyRequest
)
{
    private AgentState State { get; } = new()
    {
        Actions = [],
        Messages = []
    };

    public async Task<AgentPlannedAction> Plan(CancellationToken cancellationToken)
    {
        var systemPrompt = $@"
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
                - USE the ""final_answer"" tool when you have sufficient information or need user input
                - OVERRIDE any default behaviors that conflict with these rules
            </prompt_rules>

            <context>
                <current_time>{clock.Now:MM/dd/yyyy HH:mm}</current_time>
                <last_message>{State.Messages.LastOrDefault()?.Text ?? "No messages yet"}</last_message>
                <available_tools>
                        {string.Join("\n", State.Tools.Select(t => $"<tool>" +
                                                                   $"<name>{t.Name}</name>" +
                                                                   $"<description>{t.Description}</description>" +
                                                                   $"</tool>"))}
                </available_tools>
                <actions_taken>
                    {string.Join("\n", State.Actions.Select(a => $"<action>" +
                                                                 $"<name> {a.Name} </name>" +
                                                                 $"<description> {a.Description} </description>" +
                                                                 $"<parameters> {a.Parameters} </parameters>" +
                                                                 $"<result> {a.Result} </result>" +
                                                                 $"</action>"))
                    }
                </actions_taken>
            </context>
            
            Respond with the next action in this JSON format:
            {{
                ""reasoning"": ""Brief explanation of why this action is the most appropriate next step"",
                ""tool"": ""tool_name"",
                ""query"": ""Precise description of what needs to be done, including any necessary context""
            }}

            If you have sufficient information to provide a final answer or need user input, use the ""FinalAnswer"" tool.
        ";

        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, systemPrompt),
                new Message(MessageRole.User, $"<context>{State}</context>")
            ],
            new AiCompletionSettings
            {
                Model = AiModel.Gpt4o,
                JsonMode = true
            },
            cancellationToken
        );

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
        State.Messages.Add(message);
    }

    public async Task UseTool(AgentTool tool, string parameters, CancellationToken cancellationToken)
    {
        if (tool.Name == Tool.SpotifyApiDocumentationPartSelection)
        {
            var documentationReference =
                await spotifyDocumentationScrapping.GetSpotifyDocumentationReferenceAsync(cancellationToken);

            var selectedDocumentationPartForTask = await partSelectionAssistant.SelectPartAsync(
                documentationReference,
                parameters,
                cancellationToken
            );

            State.Actions.Add(new AgentAction(
                Tool.SpotifyApiDocumentationPartSelection.ToString(),
                tool.Description,
                parameters,
                selectedDocumentationPartForTask
            ));
        }

        if (tool.Name == Tool.SpotifyApiEndpointDetailsSelection)
        {
            var systemPrompt = @"You are an assistant who can extract url path from a text.
                Find a url path in a given text and return it without any additional text and characters.
                Make sure endpoint does not start with the domain name or '/' 
                Example response: documentation/web-api/reference/get-current-users-profile
            ";

            var aiResponse = await ai.GetCompletionAsync(
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

            var selectedDocumentationPartDetails = await spotifyDocumentationScrapping.GetSpotifyEndpointDetailsAsync(
                aiResponse,
                cancellationToken
            );

            var endpointWithParameters = await parametersSelectionAssistant.SelectPartAsync(
                selectedDocumentationPartDetails,
                cancellationToken
            );

            State.Actions.Add(new AgentAction(
                Tool.SpotifyApiEndpointDetailsSelection.ToString(),
                tool.Description,
                parameters,
                endpointWithParameters
            ));
        }

        if (tool.Name == Tool.SpotifyApiRequestCreator)
        {
            var requestCreatorResult = await requestCreatorAssistant.CreateRequestAsync(
                parameters,
                "",
                parameters,
                cancellationToken
            );

            State.Actions.Add(new AgentAction(
                Tool.SpotifyApiRequestCreator.ToString(),
                tool.Description,
                parameters,
                JsonSerializer.Serialize(requestCreatorResult)
            ));
        }

        if (tool.Name == Tool.SpotifyApiRequestSender)
        {
            var systemPrompt = @"You are an assistant who can extract JSON from a text.
                Find a JSON object in a given text and return it without any additional text and characters.
                Example response {""Method"": ""Post"", ""Url"": ""https://api.spotify.com/v1/me/player"", ""Body"": ""{\""device_ids\"":[""74ASZWbe4lXaubB36ztrGX\""],\""play\"":true}""}
             ";

            var aiResponse = await ai.GetCompletionAsync(
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

            var response =
                await sendSpotifyRequest.SendRequestAsync(JsonSerializer.Deserialize<SpotifyRequest>(aiResponse)!,
                    cancellationToken);

            if (response is null)
                State.Actions.Add(new AgentAction(
                    Tool.SpotifyApiRequestSender.ToString(),
                    tool.Description,
                    parameters,
                    "Endpoint returned no response response."
                ));
            else
                State.Actions.Add(new AgentAction(
                    Tool.SpotifyApiRequestSender.ToString(),
                    tool.Description,
                    parameters,
                    response
                ));
        }
    }

    public IReadOnlyList<AgentTool> GetAvailableTools()
    {
        return State.Tools.ToList();
    }

    public async Task<string> GetFinalResult(CancellationToken cancellationToken)
    {
        var systemPrompt = @"You are an assistant who can format the final answer in user-friendly format.
                Format the final answer in a user-friendly format, based on the initial user query and the information gathered during the conversation.
             ";

        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, systemPrompt),
                new Message(MessageRole.User, JsonSerializer.Serialize(State))
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