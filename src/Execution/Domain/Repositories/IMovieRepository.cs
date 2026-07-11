using Execution.Domain.Entities;

namespace Execution.Domain.Repositories;

/// <summary>
/// Repository interface for Movie aggregate. Follows Interface Segregation (dotnet-solid).
/// All methods accept <see cref="CancellationToken"/> per dotnet-async.
/// </summary>
public interface IMovieReader
{
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Movie?> GetByTitleAsync(string title, CancellationToken ct = default);
    Task<IReadOnlyCollection<Movie>> GetPagedAsync(
        int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
}

public interface IMovieWriter
{
    void Add(Movie movie);
    void Remove(Movie movie);
}

/// <summary>
/// Combined read/write interface for Movie. Implementation decides whether to split internally.
/// </summary>
public interface IMovieRepository : IMovieReader, IMovieWriter
{
}
