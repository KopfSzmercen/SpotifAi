using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SpotifAi.Ai.Tools;

namespace SpotifAi.Ai.Agent;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Tool
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

internal sealed record AgentAction(string Name, string Description, string Parameters, string Result);

internal class AgentState(IEnumerable<AgentTool> tools)
{
    public List<Message> Messages { get; set; } = [];

    public List<AgentAction> Actions { get; set; } = [];

    public List<AgentTool> Tools { get; set; } = tools.ToList();
}