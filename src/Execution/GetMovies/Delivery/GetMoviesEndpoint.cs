using Execution.GetMovies.Workflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net;

namespace Execution.GetMovies.Delivery;

/// <summary>
/// Endpoint for retrieving the paged movie list.
/// </summary>
public static class GetMoviesEndpoint
{
    public static RouteHandlerBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet("/getMovies", async (
                int? page,
                int? pageSize,
                GetMoviesOrchestrator orchestrator,
                HttpContext httpContext,
                CancellationToken ct) =>
            {
                var correlationId = GetCorrelationId(httpContext);
                httpContext.Response.Headers["X-Correlation-Id"] = correlationId;

                var result = await orchestrator.Execute(page ?? 1, pageSize ?? 10, ct).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    return Results.Problem(
                        title: "Unexpected error",
                        detail: result.Error.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        instance: httpContext.Request.Path,
                        extensions: new Dictionary<string, object?>
                        {
                            ["traceId"] = correlationId,
                        });
                }

                var response = result.Value!;
                return Results.Ok(new GetMoviesResponse(
                    response.Items
                        .Select(item => new GetMovieItemResponse(item.Id, item.Title, item.Genre, item.CreatedAt))
                        .ToArray(),
                    response.Page,
                    response.PageSize,
                    response.TotalCount));
            })
            .WithName("GetMovies")
            .WithSummary("Get a paged movie list")
            .WithDescription("Returns the current movie catalog sorted by newest created items first.")
            .WithTags("Execution")
            .Produces<GetMoviesResponse>((int)HttpStatusCode.OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AllowAnonymous();
    }

    private static string GetCorrelationId(HttpContext httpContext)
    {
        var headerValue = httpContext.Request.Headers["X-Correlation-Id"].FirstOrDefault();
        return string.IsNullOrWhiteSpace(headerValue) ? Guid.NewGuid().ToString("N") : headerValue;
    }
}
