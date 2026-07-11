using Execution.Domain.ValueObjects;

namespace Execution.UpdateMovie.Domain.ValueObjects;

/// <summary>
/// Result payload returned by the update-movie workflow.
/// </summary>
public sealed record UpdateMovieResult(
    Guid Id,
    string Title,
    MovieGenre Genre,
    DateTimeOffset CreatedAt);