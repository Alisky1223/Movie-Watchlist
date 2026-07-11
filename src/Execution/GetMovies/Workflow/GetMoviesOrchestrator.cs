using Execution.Domain.Repositories;
using Execution.GetMovies.Domain.ValueObjects;
using SharedKernel.Domain.ValueObjects;

namespace Execution.GetMovies.Workflow;

/// <summary>
/// Orchestrates the paged movie list query.
/// </summary>
public sealed class GetMoviesOrchestrator(IMovieRepository movieRepository)
{
    public async Task<Result<GetMoviesResult>> Execute(int page, int pageSize, CancellationToken ct)
    {
        var items = await movieRepository.GetPagedAsync(page, pageSize, ct).ConfigureAwait(false);
        var totalCount = await movieRepository.GetTotalCountAsync(ct).ConfigureAwait(false);

        return Result<GetMoviesResult>.Success(new GetMoviesResult(
            items.Select(movie => new GetMovieItemResult(
                movie.Id,
                movie.Title,
                movie.Genre.ToString(),
                movie.CreatedAt)).ToArray(),
            page,
            pageSize,
            totalCount));
    }
}