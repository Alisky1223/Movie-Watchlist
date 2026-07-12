namespace Execution.GetMovies.Delivery;

/// <summary>
/// Query parameters for the paged movie list request.
/// </summary>
public sealed record GetMoviesRequest(int Page, int PageSize);