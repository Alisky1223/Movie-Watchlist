using Execution.Domain.Entities;
using Execution.Domain.Repositories;
using Execution.Domain.ValueObjects;
using SharedKernel.Domain.ValueObjects;

namespace Execution.UpdateMovie.BusinessActions;

/// <summary>
/// Loads the target movie for update or deletion operations.
/// </summary>
public sealed class LoadMovieAction(IMovieRepository movieRepository)
{
    public async Task<Result<Movie>> Execute(Guid movieId, CancellationToken ct)
    {
        var movie = await movieRepository.GetByIdAsync(movieId, ct).ConfigureAwait(false);
        if (movie is null)
        {
            return Result<Movie>.Failure(new MovieNotFoundError(movieId));
        }

        return Result<Movie>.Success(movie);
    }
}