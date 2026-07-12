using Execution.AddMovie.Workflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Domain.ValueObjects;
using System.Net;

namespace Execution.AddMovie.Delivery
{
    /// <summary>
    /// Endpoint for creating a movie in the watchlist catalog.
    /// </summary>
    public sealed class AddMovieEndpoint
    {
        public static RouteHandlerBuilder Map(IEndpointRouteBuilder app)
        {
            return app.MapPost("/addMovies", async (
                    AddMovieRequest request,
                    AddMovieOrchestrator orchestrator,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var correlationId = GetCorrelationId(httpContext);
                    httpContext.Response.Headers["X-Correlation-Id"] = correlationId;

                    var validationResult = AddMovieValidator.Validate(request);
                    if (!validationResult.IsSuccess)
                    {
                        return Results.BadRequest(CreateProblemDetails(
                            StatusCodes.Status400BadRequest,
                            "Validation failed",
                            validationResult.Error.Message,
                            correlationId,
                            httpContext.Request.Path));
                    }

                    var command = validationResult.Value!;
                    var result = await orchestrator.Execute(command, ct).ConfigureAwait(false);
                    if (!result.IsSuccess)
                    {
                        return ToProblemResult(result.Error, correlationId, httpContext);
                    }

                    var payload = result.Value!;
                    var response = new AddMovieResponse(payload.Id, payload.Title, payload.Genre.ToString(), payload.CreatedAt);
                    return Results.Created($"/movies/{payload.Id}", response);
                })
                .WithName("AddMovie")
                .WithSummary("Add a movie to the catalog")
                .WithDescription("Creates a new movie with title and genre.")
                .WithTags("Execution")
                .Produces<AddMovieResponse>((int)HttpStatusCode.Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status409Conflict)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .AllowAnonymous();
        }

        private static IResult ToProblemResult(Error error, string correlationId, HttpContext httpContext)
        {
            return error.Code switch
            {
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
}
