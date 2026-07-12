using Execution.AddMovie.BusinessActions;
using Execution.AddMovie.Domain.ValueObjects;
using Execution.Domain.Entities;
using SharedKernel.Domain.Services;
using SharedKernel.Domain.ValueObjects;

namespace Execution.AddMovie.Workflow;

/// <summary>
/// Coordinates the add-movie use case.
/// </summary>
public sealed class AddMovieOrchestrator(
    EnsureTitleUniqueAction ensureTitleUniqueAction,
    PersistNewMovieAction persistNewMovieAction,
    IClock clock)
{
    public async Task<Result<AddMovieResult>> Execute(AddMovieCommand command, CancellationToken ct)
    {
        var titleCheck = await ensureTitleUniqueAction.Execute(command.Title, ct).ConfigureAwait(false);
        if (!titleCheck.IsSuccess)
        {
            return Result<AddMovieResult>.Failure(titleCheck.Error);
        }

        var movie = Movie.Create(command.Title, command.Genre, clock);
        await persistNewMovieAction.Execute(movie, ct).ConfigureAwait(false);

        return Result<AddMovieResult>.Success(new AddMovieResult(
            movie.Id,
            movie.Title,
            movie.Genre,
            movie.CreatedAt));
    }
}