using Execution.Domain.Entities;
using Execution.Domain.Repositories;

namespace Execution.AddMovie.BusinessActions;

/// <summary>
/// Persists a newly created movie and commits the unit of work.
/// </summary>
public sealed class PersistNewMovieAction(IMovieRepository movieRepository)
{
    public async Task Execute(Movie movie, CancellationToken ct)
    {
        movieRepository.Add(movie);
        await movieRepository.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}