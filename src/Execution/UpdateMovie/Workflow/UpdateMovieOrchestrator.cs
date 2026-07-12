using Execution.Domain.Entities;
using Execution.Domain.Repositories;
using Execution.Domain.ValueObjects;
using Execution.UpdateMovie.BusinessActions;
using Execution.UpdateMovie.Domain.ValueObjects;
using SharedKernel.Domain.ValueObjects;

namespace Execution.UpdateMovie.Workflow;

/// <summary>
/// Coordinates the update-movie use case.
/// </summary>
public sealed class UpdateMovieOrchestrator(
    LoadMovieAction loadMovieAction,
    PersistMovieAction persistMovieAction,
    IMovieRepository movieRepository)
{
    public async Task<Result<UpdateMovieResult>> Execute(UpdateMovieCommand command, CancellationToken ct)
    {
        var movieLookup = await loadMovieAction.Execute(command.MovieId, ct).ConfigureAwait(false);
        if (!movieLookup.IsSuccess)
        {
            return Result<UpdateMovieResult>.Failure(movieLookup.Error);
        }

        var movie = movieLookup.Value!;
        MovieGenre? genre = null;
        if (command.Genre is not null && MovieGenreParser.TryParse(command.Genre, out var parsedGenre))
        {
            genre = parsedGenre;
        }

        var requestedTitle = command.Title?.Trim();
        if (requestedTitle is not null)
        {
            var duplicate = await movieRepository.GetByTitleAsync(requestedTitle, ct).ConfigureAwait(false);
            if (duplicate is not null && duplicate.Id != command.MovieId)
            {
                return Result<UpdateMovieResult>.Failure(new MovieAlreadyExistsError(requestedTitle));
            }
        }

        movie.ApplyChanges(requestedTitle, genre);
        await persistMovieAction.Execute(movie, ct).ConfigureAwait(false);

        return Result<UpdateMovieResult>.Success(new UpdateMovieResult(
            movie.Id,
            movie.Title,
            movie.Genre,
            movie.CreatedAt));
    }
}