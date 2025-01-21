using SpotifAi.Ai.Agent;
using SpotifAi.Ai.Assistants.SpotifyDocumentation;
using SpotifAi.Scrapping;

namespace SpotifAi.Ai.Tools;

internal sealed class SpotifyApiDocumentationPartSelectionTool(
    SpotifyDocumentationScrapping spotifyDocumentationScrapping,
    SpotifyDocumentationPartSelectionAssistant partSelectionAssistant
) :
    AgentTool(Tool.SpotifyApiDocumentationPartSelection,
        """
        This tool has access to the Spotify API endpoints references and can select the most relevant endpoint for a given task. 
        Parameters: Description of the endpoint to search in the reference e.g: Endpoint to get the current user's profile
        Result: result is always the endpoint path, eg. /documentation/web-api/reference/get-current-users-profile
        """
    )
{
    public override async Task<string> ExecuteAsync(string parameters, CancellationToken cancellationToken)
    {
        var documentationReference =
            await spotifyDocumentationScrapping.GetSpotifyDocumentationReferenceAsync(cancellationToken);

        var selectedDocumentationPartForTask = await partSelectionAssistant.SelectPartAsync(
            documentationReference,
            parameters,
            cancellationToken
        );

        return selectedDocumentationPartForTask;
    }
}