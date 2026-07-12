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
    private readonly MovieWatchlistDbContext _context;
    private readonly DbSet<Movie> _movies;

    public MovieRepository(MovieWatchlistDbContext context)
    {
        _context = context;
        _movies = context.Movies;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, ct)
            .ConfigureAwait(false);
    }

    public async Task<Movie?> GetByTitleAsync(string title, CancellationToken ct = default)
    {
        var normalizedTitle = title.Trim().ToLowerInvariant();

        return await _movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Title.ToLower() == normalizedTitle, ct)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Movie>> GetPagedAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        var items = await _movies
            .AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return items;
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
    {
        return await _movies.CountAsync(ct).ConfigureAwait(false);
    }

    public void Add(Movie movie)
    {
        _movies.Add(movie);
    }

    public void Remove(Movie movie)
    {
        _movies.Remove(movie);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
