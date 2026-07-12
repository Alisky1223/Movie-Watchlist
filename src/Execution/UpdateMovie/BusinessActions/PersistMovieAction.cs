using Execution.Domain.Entities;
using Execution.Domain.Repositories;

namespace Execution.UpdateMovie.BusinessActions;

/// <summary>
/// Persists changes to an existing movie entity.
/// </summary>
public sealed class PersistMovieAction(IMovieRepository movieRepository)
{
    public async Task Execute(Movie movie, CancellationToken ct)
    {
        await movieRepository.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}