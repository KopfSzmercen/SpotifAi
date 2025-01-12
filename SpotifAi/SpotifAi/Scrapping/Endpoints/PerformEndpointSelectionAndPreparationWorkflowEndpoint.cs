using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SpotifAi.Ai.Assistants.SpotifyDocumentation;
using SpotifAi.Spotify.Api;

namespace SpotifAi.Scrapping.Endpoints;

internal static class PerformEndpointSelectionAndPreparationWorkflowEndpoint
{
    public static RouteGroupBuilder RegisterPerformEndpointSelectionAndPreparationWorkflowEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost("perform", PerformEndpointSelectionAndPreparationWorkflow)
            .WithDescription("Performs the endpoint selection and preparation workflow.");

        return group;
    }

    private static async Task<Results<
            BadRequest<string>,
            Ok<string>>>
        PerformEndpointSelectionAndPreparationWorkflow(
            [FromBody] Request request,
            [FromServices] SpotifyDocumentationPartSelectionAssistant partSelectionAssistant,
            [FromServices] SpotifyEndpointParametersSelectionAssistant parametersSelectionAssistant,
            [FromServices] SpotifyRequestCreatorAssistant requestCreatorAssistant,
            [FromServices] SpotifyDocumentationScrapping spotifyDocumentationScrapping,
            [FromServices] SendSpotifyRequest sendSpotifyRequest,
            [FromServices] SpotifyResponsePrettifierAssistant responsePrettifierAssistant,
            CancellationToken cancellationToken
        )
    {
        var documentationReference =
            await spotifyDocumentationScrapping.GetSpotifyDocumentationReferenceAsync(cancellationToken);

        var selectedDocumentationPartForTask = await partSelectionAssistant.SelectPartAsync(
            documentationReference,
            request.Command,
            cancellationToken
        );

        var selectedDocumentationPartDetails = await spotifyDocumentationScrapping.GetSpotifyEndpointDetailsAsync(
            selectedDocumentationPartForTask,
            cancellationToken
        );

        var endpointWithParameters = await parametersSelectionAssistant.SelectPartAsync(
            selectedDocumentationPartDetails,
            cancellationToken
        );

        var requestCreatorResult = await requestCreatorAssistant.CreateRequestAsync(
            request.Command,
            "",
            endpointWithParameters,
            cancellationToken
        );

        var response = await sendSpotifyRequest.SendRequestAsync(requestCreatorResult, cancellationToken);

        if (response is null)
            return TypedResults.Ok("No response.");

        response = await responsePrettifierAssistant.PrettifyResponseAsync(response, request.Command,
            cancellationToken);

        return TypedResults.Ok(response);
    }

    private sealed record Request(string Command);
}