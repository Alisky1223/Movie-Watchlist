using System.Net;
using Execution.DeleteMovie.Workflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Domain.ValueObjects;

namespace Execution.DeleteMovie.Delivery;

/// <summary>
/// Endpoint for deleting an existing movie.
/// </summary>
public static class DeleteMovieEndpoint
{
    public static RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapDelete("/movies/{movieId:guid}", async (
                Guid movieId,
                DeleteMovieOrchestrator orchestrator,
                HttpContext httpContext,
                CancellationToken ct) =>
            {
                var correlationId = GetCorrelationId(httpContext);
                httpContext.Response.Headers["X-Correlation-Id"] = correlationId;

                var result = await orchestrator.Execute(movieId, ct).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    return Results.NotFound(CreateProblemDetails(
                        StatusCodes.Status404NotFound,
                        "Not Found",
                        result.Error.Message,
                        correlationId,
                        httpContext.Request.Path));
                }

                return Results.NoContent();
            })
            .WithName("DeleteMovie")
            .WithSummary("Delete an existing movie")
            .WithDescription("Deletes a movie from the catalog.")
            .WithTags("Execution")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AllowAnonymous();
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