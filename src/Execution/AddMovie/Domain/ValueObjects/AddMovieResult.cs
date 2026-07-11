using Execution.Domain.ValueObjects;

namespace Execution.AddMovie.Domain.ValueObjects;

/// <summary>
/// Result payload returned by the add-movie workflow.
/// </summary>
public sealed record AddMovieResult(
    Guid Id,
    string Title,
    MovieGenre Genre,
    DateTimeOffset CreatedAt);