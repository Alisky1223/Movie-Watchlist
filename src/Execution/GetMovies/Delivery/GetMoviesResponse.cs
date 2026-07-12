namespace Execution.GetMovies.Delivery;

/// <summary>
/// Response payload for the paged movie list query.
/// </summary>
public sealed record GetMoviesResponse(
    IReadOnlyCollection<GetMovieItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);

public sealed record GetMovieItemResponse(
    Guid Id,
    string Title,
    string Genre,
    DateTimeOffset CreatedAt);