using System.ComponentModel.DataAnnotations;

namespace Execution.AddMovie.Delivery;

/// <summary>
/// Request payload for adding a movie.
/// </summary>
public sealed record AddMovieRequest(
    [property: Required, MinLength(1), MaxLength(200)] string Title,
    [property: Required] string Genre);