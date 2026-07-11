using Execution.Domain.Entities;
using Execution.Domain.Repositories;
using Execution.Domain.ValueObjects;
using SharedKernel.Domain.ValueObjects;

namespace Execution.AddMovie.BusinessActions;

/// <summary>
/// Ensures that a movie title is globally unique before inserting it.
/// </summary>
public sealed class EnsureTitleUniqueAction(IMovieRepository movieRepository)
{
    public async Task<Result> Execute(string title, CancellationToken ct)
    {
        var duplicate = await movieRepository.GetByTitleAsync(title, ct).ConfigureAwait(false);
        if (duplicate is not null)
        {
            return Result.Failure(new MovieAlreadyExistsError(title));
        }

        return Result.Success();
    }
}