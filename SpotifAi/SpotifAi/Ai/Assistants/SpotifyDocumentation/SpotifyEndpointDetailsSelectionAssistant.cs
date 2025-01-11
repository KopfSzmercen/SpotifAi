namespace SpotifAi.Ai.Assistants.SpotifyDocumentation;

internal sealed class SpotifyEndpointParametersSelectionAssistant(IAi ai)
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
                                    <description>An ISO 3166-1 alpha-2 country code. If a country code is specified, only content that is available in that market will be returned. </description>
                                </param>
                                <param>
                                    <name>limit</name>
                                    <type>integer</type>
                                    <description>The maximum number of items to return. Default: 20. Minimum: 1. Maximum: 50.  </description>
                                </param>                                            
                            </query_params>

                           <route_params>
                                <param>
                                    <name>id</name>
                                    <type>string</type>
                                    <description>The Spotify ID for the show. </description>
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
                            </output>   
                </example_2>

            </examples>
     ";

    public async Task<string> SelectPartAsync(string endpointDetails, CancellationToken cancellationToken)
    {
        var aiResponse = await ai.GetCompletionAsync(
            [
                new Message(MessageRole.System, SystemPrompt),
                new Message(MessageRole.User,
                    $@"
                    <endpoint_details>
                    {endpointDetails}
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

        return aiResponse;
    }
}