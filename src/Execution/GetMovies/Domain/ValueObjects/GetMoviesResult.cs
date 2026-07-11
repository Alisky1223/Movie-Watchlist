namespace Execution.GetMovies.Domain.ValueObjects;

/// <summary>
/// Workflow result for the paged movie list route.
/// </summary>
public sealed record GetMoviesResult(
    IReadOnlyCollection<GetMovieItemResult> Items,
    int Page,
    int PageSize,
    int TotalCount);

public sealed record GetMovieItemResult(
    Guid Id,
    string Title,
    string Genre,
    DateTimeOffset CreatedAt);