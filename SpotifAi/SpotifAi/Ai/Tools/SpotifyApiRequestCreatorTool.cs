using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SpotifAi.Ai.Agent;
using SpotifAi.Persistence;
using SpotifAi.Spotify.Api;

namespace SpotifAi.Ai.Tools;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HttpMethod
{
    [EnumMember(Value = "Get")] Get,

    [EnumMember(Value = "Post")] Post,

    [EnumMember(Value = "Put")] Put,

    [EnumMember(Value = "Delete")] Delete
}

public sealed record SpotifyRequest(HttpMethod Method, string Url, string? Body);

public sealed record PreparedSpotifyRequest(HttpMethod Method, string Url, string? Body, string? MissingMetadata);

internal sealed class SpotifyApiRequestCreatorTool(
    SendSpotifyRequest sendSpotifyRequest,
    IAi ai,
    IServiceScopeFactory serviceScopeFactory
) : AgentTool(
    Tool.SpotifyApiRequestCreator,
    """
    <tool_description>
        This tool can create and send a request for a specific Spotify API endpoint. 
        It uses the endpoint details saved by other tools to know what parameters are available for the endpoint.
        It uses task description, metadata, and endpoint details to create a request.
        If parameters like ids are missing, try to find them using other endpoints and reuse them in the next call for this tool.
    </tool_description>

    <tool_parameters>
        - documentation endpoint path without domain e.g: documentation/web-api/reference/get-current-users-profile.
        - metadata: optional, unstructured parameters with the context of the request, required ids, search queries, etc.
        - task: the task to perform with the request.
        All tool parameters should be in a string separated by a comma.
    </tool_parameters>

    <tool_result>
        Result of the request.
    </tool_result>

    <tool_example>
        <input>
               endpoint_path: documentation/web-api/reference/get-current-users-profile, metadata: userId: 123, task: Get the current user's profile
        </input>
    
        <output>
            {
                "Result": "Request result"
            }
        </output>
    </tool_example>
    """
)
{
    private const string SystemPrompt = @" 
           From now on are a helpful assistant who can analyze endpoint specification, metadata, and task to create a Spotify request.
            
            <prompt_objective>
                You are going to be given a task to perform, metadata, and endpoint specification.
                You have to analyze the task, metadata, and endpoint specification to create and create a request to Spotify API.
                Always return the request in JSON format:
                {
                    ""Reasoning"": ""Explain what parameters you used and why"",
                    ""Method"": ""HttpMethodToUse: Get, Post, Put, Delete"",
                    ""Url"": ""UrlToUse, with query params if needed"",
                    ""Body"": ""BodyToUse if needed""
                    ""MissingMetadata"": ""a short description of missing metadata which is required for the request. If no metadata is missing, return null""
                }
            </prompt_objective>
            
            <prompt_example>
                   <task>Start playback on a new device</task>
                   <endpoint_details>
                                   <request_body_params>
                                        <param>
                                            <name>device_ids</name>
                                            <type>array of strings</type>
                                            <description>A JSON array containing the ID of the device on which playback should be started/transferred. </description>
                                            <example>{device_ids:[""74ASZWbe4lXaubB36ztrGX""]}</example>
                                        </param>
                                        <param>
                                            <name>play</name>
                                            <type>boolean</type>
                                            <description>true: ensure playback happens on new device. false or not provided: keep the current playback state. </description>
                                            <example>true</example>
                                        </param>
                                    </request_body_params>
                                    
                             
                                     <request_sample>
                                          https://api.spotify.com/v1/me/player
                                          {
                                              ""device_ids"":[""74ASZWbe4lXaubB36ztrGX""],
                                              ""play"":true
                                          }
                                     </request_sample>
                    </endpoint_details>
                    <metadata>userDeviceId: 74ASZWbe4lXaubB36ztrGX</metadata>
                    
                    <result> ""{
                                  ""Method"": ""Post"",
                                  ""Url"": ""https://api.spotify.com/v1/me/player"",
                                  ""Body"": ""{\""device_ids\"":[""74ASZWbe4lXaubB36ztrGX\""],\""play\"":true}""
                                  ""MissingMetadata"": null
                              }""
                    </result>
            </prompt_example>
       ";

    public override async Task<string> ExecuteAsync(string parameters, CancellationToken cancellationToken)
    {
        var requestParameters = await PrepareParametersAsync(parameters, cancellationToken);

        var endpointDetails = await GetEndpointDetailsFromCacheAsync(requestParameters.EndpointPath, cancellationToken);

        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, SystemPrompt),
                new Message(MessageRole.User,
                    $@"
                    <task>{requestParameters.Task}</task>

                    <endpoint_details>
                    {endpointDetails}
                    </endpoint_details>

                    <metadata>{requestParameters.Metadata}</metadata>"
                )
            ],
            new AiCompletionSettings
            {
                Model = AiModel.Gpt4OMini,
                JsonMode = true
            },
            cancellationToken
        );

        var request = JsonSerializer.Deserialize<PreparedSpotifyRequest>(aiResponse);

        if (request is null)
            throw new Exception("Failed to deserialize PreparedSpotifyRequest");

        if (request.MissingMetadata is not null) return request.MissingMetadata;

        var response =
            await sendSpotifyRequest.SendRequestAsync(
                new SpotifyRequest(request.Method, request.Url, request.Body),
                cancellationToken
            );

        return response ?? "Action returned no response";
    }

    private async Task<CreateRequestParameters> PrepareParametersAsync(string parameters,
        CancellationToken cancellationToken)
    {
        const string systemPrompt =
            @"From now on you are an assistant who can analyze given input and structure it to a valid JSON format.
                
                        <prompt_objective>
                                You are going to be given parameters in string form.
                                Your task is to structure them into a valid JSON format: 
                                {
                                    ""EndpointPath"": ""required endpoint path"",
                                    ""Metadata"": ""metadata"",
                                    ""Task"": ""task to perform""
                                }
                        </prompt_objective>

                        <prompt_rules>
                                - Always return the parameters in the required JSON format:   {
                                    ""EndpointPath"": ""required endpoint path"",
                                    ""Metadata"": ""metadata"",
                                    ""Task"": ""task to perform""
                                }
                                - The result must not have any additional characters, only the JSON format.
                        </prompt_rules>

                        <prompt_examples>
                               <input>
                                        <metadata>
                                            userId: 123
                                            showId: 1245
                                         </metadata>
                                        <endpoint_path>documentation/web-api/reference/get-current-users-profile</endpoint_path>
                                        <task>Get an album with show 12345</task>
                               </input>
                                
                                <output>
                                    {
                                        ""EndpointPath"": ""documentation/web-api/reference/get-current-users-profile"",
                                        ""Metadata"": ""userId: 123 showId: 1245"",
                                        ""Task"": ""Get an album with show 12345""
                                    }
                                </output>
                        </prompt_examples>
        ";

        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, systemPrompt),
                new Message(MessageRole.User, parameters)
            ],
            new AiCompletionSettings
            {
                Model = AiModel.Gpt4OMini,
                JsonMode = true
            },
            cancellationToken
        );

        return JsonSerializer.Deserialize<CreateRequestParameters>(aiResponse) ??
               throw new Exception("Failed to deserialize CreateRequestParameters");
    }

    private async Task<string> GetEndpointDetailsFromCacheAsync(string endpoint, CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var endpointDetailsCached = await appDbContext.EndpointDetails
            .Where(x => x.EndpointPath == endpoint)
            .FirstOrDefaultAsync(cancellationToken);

        return endpointDetailsCached?.Details ?? throw new Exception($"{endpoint} Endpoint details not found");
    }

    private sealed record CreateRequestParameters(string EndpointPath, string Metadata, string Task);
}