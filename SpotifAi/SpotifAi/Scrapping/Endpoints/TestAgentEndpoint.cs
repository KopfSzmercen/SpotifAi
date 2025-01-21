using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SpotifAi.Ai;
using SpotifAi.Ai.Agent;

namespace SpotifAi.Scrapping.Endpoints;

internal static class TestAgentEndpoint
{
    private const int MaxIterations = 10;

    public static RouteGroupBuilder RegisterTestAgentEndpoint(
        this RouteGroupBuilder group)
    {
        group.MapPost("test", TestAgent)
            .RequireAuthorization()
            .WithDescription("Tests the agent.");

        return group;
    }

    private static async Task<Results<
            BadRequest<string>,
            Ok<string>>>
        TestAgent(
            [FromBody] Request request,
            [FromServices] Agent agent,
            CancellationToken cancellationToken
        )
    {
        agent.AddMessage(new Message(MessageRole.User, request.Command));

        for (var i = 0; i <= MaxIterations; i++)
        {
            var nextMove = await agent.Plan(cancellationToken);

            var tool = agent
                .GetAvailableTools()
                .SingleOrDefault(x => x.Name == nextMove.Tool);

            if (tool == null) break;

            await agent.UseTool(tool, nextMove.Query, cancellationToken);

            if (tool.Name != Tool.FinalAnswer) continue;
            await agent.UseTool(tool, agent.GetState(), cancellationToken);
            break;
        }

        var answer = agent.GetLastResult();

        return TypedResults.Ok(answer);
    }

    private sealed record Request(string Command);
}