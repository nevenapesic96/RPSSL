using Api.Configuration;
using Application.Common;
using Application.DTOs;
using Application.Services;

namespace Api;

public static class Endpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/choices", (IChoicesService choicesService) =>
            {
                var result = choicesService.GetAllChoices();

                return Results.Ok(result);
            })
            .Produces<IEnumerable<ChoiceResponse>>();

        app.MapGet("/random", async (IChoicesService choiceService, CancellationToken cancellationToken) =>
            {
                var result = await choiceService.GetRandomChoice(cancellationToken);

                return Results.Ok(result);
            })
            .Produces<ChoiceResponse>();

        app.MapPost("/play", async (PlayRequest request, IPlayService playService, CancellationToken cancellationToken) =>
                {
                    var result = await playService.Play(request, cancellationToken);

                    return result.IsSuccessful
                        ? Results.Ok(result)
                        : Results.UnprocessableEntity("Unable to save play result on scoreboard");
                })
            .RequireValidation<PlayRequest>()
            .ProducesValidationProblem()
            .Produces<PlayResponse>();

        app.MapDelete("/play", async (string? username, IPlayService playService, CancellationToken cancellationToken) =>
            {
                var result = await playService.ResetResults(username, cancellationToken);
                if (result.IsSuccessful) return Results.Ok();

                var error = result.Error!;

                return error.Type switch
                {
                    ApplicationErrorType.NotFound => Results.BadRequest("Username not found"),
                    ApplicationErrorType.UnprocessableEntity => Results.UnprocessableEntity("Unable to delete all results"),
                    _ => throw new ArgumentOutOfRangeException()
                };
            });

        app.MapGet("/results", async (IPlayService playService, CancellationToken cancellationToken) =>
            {
                var results = await playService.GetLatestResults(cancellationToken);

                return Results.Ok(results);
            })
            .Produces<IEnumerable<ResultResponse>>();
    }
}