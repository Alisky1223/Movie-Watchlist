using Execution.Domain.ValueObjects;

namespace Execution.UpdateMovie.Domain.ValueObjects;

/// <summary>
/// Command for updating an existing movie.
/// </summary>
public sealed record UpdateMovieCommand(Guid MovieId, string? Title, string? Genre);