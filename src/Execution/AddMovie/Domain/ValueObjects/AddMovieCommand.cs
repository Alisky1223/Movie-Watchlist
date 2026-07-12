using Execution.Domain.ValueObjects;

namespace Execution.AddMovie.Domain.ValueObjects;

/// <summary>
/// Command for creating a new movie.
/// </summary>
public sealed record AddMovieCommand(string Title, MovieGenre Genre);