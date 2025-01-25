using Microsoft.EntityFrameworkCore;
using SpotifAi.Ai.Agent;
using SpotifAi.Persistence;
using SpotifAi.Scrapping;
using SpotifAi.SpotifyDocumentation;
using SpotifAi.Utils;

namespace SpotifAi.Ai.Tools;

internal sealed class SpotifyApiEndpointDetailsSelectionTool(
    IAi ai,
    SpotifyDocumentationScrapping spotifyDocumentationScrapping,
    IServiceScopeFactory serviceScopeFactory,
    IClock clock
) : AgentTool(Tool.SpotifyApiEndpointDetailsSelection,
    """
    <tool_description>
        This tool has access to details a specific Spotify API endpoint. It analyzes what parameters are available for the endpoint, such as query parameters, path parameters, and request body parameters.
        It returns the extracted parameters in a structured format with their name, type, description, whether they are required or optional, and an example value.
    </tool_description>

    <tool_parameters>
        Endpoint path without domain e.g: documentation/web-api/reference/get-current-users-profile
    </tool_parameters>

    <tool_result>
        Result: extracted parameters in a structured format.
    </tool_result>

    <tool_action_example>
        InputParameter: documentation/web-api/reference/get-current-users-profile
        OutputResult: [Structured parameters of the endpoint with details]
    </tool_action_example>
    """
)
{
    private const string SystemPrompt = @"
          You are a helpful assistant who can extract relevant info about a Spotify Api endpoint from a markdown file.

           <prompt_objective>
                You are going to be given a markdown file that contains information about a Spotify API endpoint.
                Your task is to find possible endpoint parameters such as query parameters, path parameters, and request body parameters.
           </prompt_objective>
           
            <rules>
                Focus only on the parameters of the endpoint, not on what the endpoint does.
                If you find a parameter, you should provide the name of the parameter and its type, description, whether it is required or optional and an example value.
                Every time you find request sample, ignore authentication headers, focus just on the structure of the request.
            </rules>
            
            <examples>
                <example_1>
                    <endpoint_details>
                    id: string Required The Spotify ID for the show.
                    Example: 38bS44xjbVVZ3No3ByF1dJ

                    market
                    string
                    An ISO 3166-1 alpha-2 country code. If a country code is specified, only content that is available in that market will be returned.
                    If a valid user access token is specified in the request header, the country associated with the user account will take priority over this parameter.
                    Note: If neither market or user country are provided, the content is considered unavailable for the client.
                    Users can view the country that is associated with their account in the account settings.
                    Example: market=ES
                    
                    limit
                    integer
                    The maximum number of items to return. Default: 20. Minimum: 1. Maximum: 50.
                    Default: limit=20
                    Range: 0 - 50
                    Example: limit=10

                    curl --request GET \
                    --url https://api.spotify.com/v1/albums/4aawyAB9vmqN3uQ7FjRGTy \
                    --header 'Authorization: Bearer 1POdFZRZbvb...qqillRxMr2z'
                    </endpoint_details>

                    <output>
                            <query_params>
                                <param>
                                    <name>market</name>
                                    <type>string</type>
                                    <required>false</required>
                                    <description>An ISO 3166-1 alpha-2 country code. If a country code is specified, only content that is available in that market will be returned. </description>
                                </param>
                                <param>
                                    <name>limit</name>
                                    <type>integer</type>
                                    <required>false</required>
                                    <description>The maximum number of items to return. Default: 20. Minimum: 1. Maximum: 50.  </description>
                                </param>                                            
                            </query_params>

                           <route_params>
                                <param>
                                    <name>id</name>
                                    <type>string</type>
                                    <description>The Spotify ID for the show. </description>
                                    <required>true</required>
                                </param>
                            </route_params>

                            <request_sample>
                                https://api.spotify.com/v1/albums/4aawyAB9vmqN3uQ7FjRGTy
                            </request_sample>
                    </output>
                </example_1>

                <example_2>
                         <endpoint_details>
                            Body application/json
                            supports free form additional properties
                            device_ids
                            array of strings
                            Required
                            A JSON array containing the ID of the device on which playback should be started/transferred.
                            For example:{device_ids:[""74ASZWbe4lXaubB36ztrGX""]}
                            Note: Although an array is accepted, only a single device_id is currently supported. Supplying more than one will return 400 Bad Request

                            play
                            boolean
                            true: ensure playback happens on new device.
                            false or not provided: keep the current playback state.    
                         </endpoint_details>

                            <output>
                                <request_body_params>
                                    <param>
                                        <name>device_ids</name>
                                        <type>array of strings</type>
                                        <required>true</required>                   
                                        <description>A JSON array containing the ID of the device on which playback should be started/transferred. Always use only one device_id, more ids will cause 400 Bad Request</description>
                                        <example>{device_ids:[""74ASZWbe4lXaubB36ztrGX""]}</example>
                                    </param>
                                    <param>
                                        <name>play</name>
                                        <type>boolean</type>
                                        <required>false</required>   
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
                            </output>   
                </example_2>
            </examples>
     ";

    public override async Task<string> ExecuteAsync(string endpointPath, CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var endpointDetailsCached = await appDbContext.EndpointDetails
            .Where(x => x.EndpointPath == endpointPath)
            .FirstOrDefaultAsync(cancellationToken);

        if (endpointDetailsCached is not null)
            return endpointDetailsCached.Details;

        var selectedDocumentationPartDetails = await spotifyDocumentationScrapping.GetSpotifyEndpointDetailsAsync(
            endpointPath,
            cancellationToken
        );

        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, SystemPrompt),
                new Message(MessageRole.User,
                    $@"
                    <endpoint_details>
                    {selectedDocumentationPartDetails}
                    </endpoint_details>
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

        var endpointDetailsToCache = new EndpointDetails
        {
            EndpointPath = endpointPath,
            Details = aiResponse,
            UpdatedAt = clock.Now
        };

        appDbContext.EndpointDetails.Add(endpointDetailsToCache);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return aiResponse;
    }
}