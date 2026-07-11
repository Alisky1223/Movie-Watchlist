namespace Execution.UpdateMovie.Delivery;

/// <summary>
/// Response payload for updating a movie.
/// </summary>
public sealed record UpdateMovieResponse(
    Guid Id,
    string Title,
    string Genre,
    DateTimeOffset CreatedAt);