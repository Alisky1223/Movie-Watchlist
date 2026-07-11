using Execution.Domain.Entities;
using Execution.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IMovieRepository"/>.
/// Uses parameterized queries (dotnet-security) and AsNoTracking for reads (dotnet-performance).
/// </summary>
public sealed class MovieRepository : IMovieRepository
{
    private readonly DbSet<Movie> _movies;

    public MovieRepository(DbSet<Movie> movies)
    {
        _movies = movies;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, ct);
    }

    public async Task<Movie?> GetByTitleAsync(string title, CancellationToken ct = default)
    {
        return await _movies
            .FirstOrDefaultAsync(m => m.Title == title, ct);
    }

    public async Task<IReadOnlyCollection<Movie>> GetPagedAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        var items = await _movies
            .AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return items;
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
    {
        return await _movies.CountAsync(ct);
    }

    public void Add(Movie movie)
    {
        _movies.Add(movie);
    }

    public void Remove(Movie movie)
    {
        _movies.Remove(movie);
    }
}
