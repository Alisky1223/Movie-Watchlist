using Execution.Domain.Entities;
using Execution.Domain.Repositories;

namespace Execution.DeleteMovie.BusinessActions;

/// <summary>
/// Removes an existing movie aggregate from the repository.
/// </summary>
public sealed class DeleteMovieAction(IMovieRepository movieRepository)
{
    public async Task Execute(Movie movie, CancellationToken ct)
    {
        movieRepository.Remove(movie);
        await movieRepository.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}