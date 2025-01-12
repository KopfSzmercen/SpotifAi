using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SpotifAi.Ai.Agent;

[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum Tool
{
    [EnumMember(Value = "SpotifyApiDocumentationPartSelection")]
    SpotifyApiDocumentationPartSelection,

    [EnumMember(Value = "SpotifyApiEndpointDetailsSelection")]
    SpotifyApiEndpointDetailsSelection,

    [EnumMember(Value = "SpotifyApiRequestCreator")]
    SpotifyApiRequestCreator,

    [EnumMember(Value = "SpotifyApiRequestSender")]
    SpotifyApiRequestSender,

    [EnumMember(Value = "FinalAnswer")] FinalAnswer
}

internal record AgentTool(Tool Name, string Description);

internal sealed record SpotifyApiDocumentationPartSelectionTool() :
    AgentTool(Tool.SpotifyApiDocumentationPartSelection,
        """
        This tool has access to the Spotify API endpoints references and can select the most relevant endpoint for a given task. 
        Parameters: Description of the endpoint to search in the reference e.g: Endpoint to get the current user's profile
        Result: result is always the endpoint path, eg. /documentation/web-api/reference/get-current-users-profile
        """
    );

internal sealed record SpotifyApiEndpointDetailsSelectionTool() : AgentTool(Tool.SpotifyApiEndpointDetailsSelection,
    """
    This tool has access to details a specific Spotify API endpoint. It analyzes what parameters are available for the endpoint, such as query parameters, path parameters, and request body parameters.
    Parameters: url for the documentation e.g: /documentation/web-api/reference/get-current-users-profile
    The tool will return the extracted parameters in a structured format.
    """
);

internal sealed record SpotifyApiRequestCreatorTool() : AgentTool(Tool.SpotifyApiRequestCreator,
    """
    This tool can create a request for a specific Spotify API endpoint. It uses the endpoint path and parameters to generate a request object. 
    Parameters: endpoint specification, metadata, and task
    Result: JSON object with the request method, url, and body
    """
);

internal sealed record SpotifyApiRequestSenderTool() : AgentTool(Tool.SpotifyApiRequestSender,
    """
    This tool can send a request to the Spotify API using the provided request object. It sends the request to the Spotify API and returns the response object. 
    Parameters: JSON object with the request method, url, and body
    Result: JSON object with the response from the Spotify API
    """
);

internal sealed record AgentAction(string Name, string Description, string Parameters, string Result);

internal class AgentState
{
    public AgentState()
    {
        Tools =
        [
            new SpotifyApiDocumentationPartSelectionTool(),
            new SpotifyApiEndpointDetailsSelectionTool(),
            new SpotifyApiRequestCreatorTool(),
            new SpotifyApiRequestSenderTool()
        ];

        Actions = [];
        Messages = [];
    }

    public required List<Message> Messages { get; set; } = [];

    public List<AgentTool> Tools { get; set; } = [];

    public required List<AgentAction> Actions { get; set; } = [];
}