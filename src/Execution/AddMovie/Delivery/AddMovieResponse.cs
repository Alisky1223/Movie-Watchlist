namespace Execution.AddMovie.Delivery;

/// <summary>
/// Response payload for a successful movie creation.
/// </summary>
public sealed record AddMovieResponse(
    Guid Id,
    string Title,
    string Genre,
    DateTimeOffset CreatedAt);