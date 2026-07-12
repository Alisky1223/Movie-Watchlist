using Execution.DeleteMovie.BusinessActions;
using Execution.Domain.Entities;
using Execution.UpdateMovie.BusinessActions;
using SharedKernel.Domain.ValueObjects;

namespace Execution.DeleteMovie.Workflow;

/// <summary>
/// Coordinates the delete-movie use case.
/// </summary>
public sealed class DeleteMovieOrchestrator(
    LoadMovieAction loadMovieAction,
    DeleteMovieAction deleteMovieAction)
{
    public async Task<Result> Execute(Guid movieId, CancellationToken ct)
    {
        var movieLookup = await loadMovieAction.Execute(movieId, ct).ConfigureAwait(false);
        if (!movieLookup.IsSuccess)
        {
            return Result.Failure(movieLookup.Error);
        }

        var movie = movieLookup.Value!;
        await deleteMovieAction.Execute(movie, ct).ConfigureAwait(false);
        return Result.Success();
    }
}