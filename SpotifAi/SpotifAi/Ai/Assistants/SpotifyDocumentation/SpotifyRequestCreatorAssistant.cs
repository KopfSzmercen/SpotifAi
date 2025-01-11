using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpotifAi.Ai.Assistants.SpotifyDocumentation;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HttpMethod
{
    [EnumMember(Value = "Get")] Get,

    [EnumMember(Value = "Post")] Post,

    [EnumMember(Value = "Put")] Put,

    [EnumMember(Value = "Delete")] Delete
}

public sealed record SpotifyRequest(HttpMethod Method, string Url, string? Body);

internal sealed class SpotifyRequestCreatorAssistant(IAi ai)
{
    private const string SystemPrompt = @" 
           You are a helpful assistant who can analyze endpoint specification, metadata, and task to create a Spotify request.
            
            <prompt_objective>
                You are going to be given a task to perform, metadata, and endpoint specification.
                You have to analyze the task, metadata, and endpoint specification to create and create a request to Spotify API.
                Always return the request in JSON format:
                {
                    ""Method"": ""HttpMethodToUse: Get, Post, Put, Delete"",
                    ""Url"": ""UrlToUse, with query params if needed"",
                    ""Body"": ""BodyToUse if needed""
                }
            </prompt_objective>
            
            <prompt_example>
                   <task>Start playback on a new device</task>
                   <endpoint_specification>
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
                    </endpoint_specification>
                    <metadata>userDeviceId: 74ASZWbe4lXaubB36ztrGX</metadata>

                     Result: ""{
                                  ""Method"": ""Post"",
                                  ""Url"": ""https://api.spotify.com/v1/me/player"",
                                  ""Body"": ""{\""device_ids\"":[""74ASZWbe4lXaubB36ztrGX\""],\""play\"":true}""
                              }""
            </prompt_example>
       ";

    public async Task<SpotifyRequest> CreateRequestAsync(string task, string metadata, string endpointSpecification,
        CancellationToken cancellationToken)
    {
        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, SystemPrompt),
                new Message(MessageRole.User,
                    $@"
                <task>{task}</task>
                <endpoint_specification>
                {endpointSpecification}
                </endpoint_specification>
                <metadata>{metadata}</metadata>
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

        var request = JsonSerializer.Deserialize<SpotifyRequest>(aiResponse);

        return request ?? throw new InvalidOperationException("Failed to create a request.");
    }
}