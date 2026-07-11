namespace Execution.UpdateMovie.Delivery;

/// <summary>
/// Request payload for updating a movie.
/// </summary>
public sealed record UpdateMovieRequest(string? Title, string? Genre);