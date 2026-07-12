using Execution.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

/// <summary>
/// Central EF Core DbContext shell. Each bounded context adds its own configurations here.
/// API does NOT auto-migrate — migrations run manually from the Migrations project.
/// </summary>
public sealed class MovieWatchlistDbContext : DbContext
{
    public DbSet<Movie> Movies => Set<Movie>();

    public MovieWatchlistDbContext(DbContextOptions<MovieWatchlistDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieWatchlistDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
