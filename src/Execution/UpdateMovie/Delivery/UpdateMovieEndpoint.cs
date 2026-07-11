using System.Net;
using Execution.Domain.ValueObjects;
using Execution.UpdateMovie.BusinessActions;
using Execution.UpdateMovie.Domain.ValueObjects;
using Execution.UpdateMovie.Workflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Domain.ValueObjects;

namespace Execution.UpdateMovie.Delivery;

/// <summary>
/// Endpoint for updating an existing movie.
/// </summary>
public static class UpdateMovieEndpoint
{
    public static RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPut("/movies/{movieId:guid}", async (
                Guid movieId,
                UpdateMovieRequest request,
                UpdateMovieOrchestrator orchestrator,
                HttpContext httpContext,
                CancellationToken ct) =>
            {
                var correlationId = GetCorrelationId(httpContext);
                httpContext.Response.Headers["X-Correlation-Id"] = correlationId;

                var validationResult = UpdateMovieValidator.Validate(request);
                if (!validationResult.IsSuccess)
                {
                    return Results.BadRequest(CreateProblemDetails(
                        StatusCodes.Status400BadRequest,
                        "Validation failed",
                        validationResult.Error.Message,
                        correlationId,
                        httpContext.Request.Path));
                }

                var result = await orchestrator.Execute(new UpdateMovieCommand(movieId, request.Title, request.Genre), ct).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    return ToProblemResult(result.Error, correlationId, httpContext);
                }

                var payload = result.Value!;
                return Results.Ok(new UpdateMovieResponse(payload.Id, payload.Title, payload.Genre.ToString(), payload.CreatedAt));
            })
            .WithName("UpdateMovie")
            .WithSummary("Update an existing movie")
            .WithDescription("Updates title and/or genre for an existing movie.")
            .WithTags("Execution")
            .Produces<UpdateMovieResponse>((int)HttpStatusCode.OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AllowAnonymous();
    }

    private static IResult ToProblemResult(Error error, string correlationId, HttpContext httpContext)
    {
        return error.Code switch
        {
            "MovieNotFound" => Results.NotFound(CreateProblemDetails(
                StatusCodes.Status404NotFound,
                "Not Found",
                error.Message,
                correlationId,
                httpContext.Request.Path)),
            "MovieAlreadyExists" => Results.Conflict(CreateProblemDetails(
                StatusCodes.Status409Conflict,
                "Conflict",
                error.Message,
                correlationId,
                httpContext.Request.Path)),
            _ => Results.Problem(
                title: "Unexpected error",
                detail: error.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                instance: httpContext.Request.Path,
                extensions: new Dictionary<string, object?>
                {
                    ["traceId"] = correlationId,
                }),
        };
    }

    private static ProblemDetails CreateProblemDetails(
        int statusCode,
        string title,
        string detail,
        string correlationId,
        string instance)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = instance,
            Extensions = { ["traceId"] = correlationId },
        };
    }

    private static string GetCorrelationId(HttpContext httpContext)
    {
        var headerValue = httpContext.Request.Headers["X-Correlation-Id"].FirstOrDefault();
        return string.IsNullOrWhiteSpace(headerValue) ? Guid.NewGuid().ToString("N") : headerValue;
    }
}