using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Persistence;

namespace Migrations;

/// <summary>
/// Design-time factory for EF Core tooling (dotnet ef migrations add ...).
/// Connection string comes from env var or falls back to a local dev placeholder.
/// </summary>
public sealed class MovieWatchlistDbContextFactory : IDesignTimeDbContextFactory<MovieWatchlistDbContext>
{
    public MovieWatchlistDbContext CreateDbContext(string[] args)
    {
        var connectionString = GetConnectionString(args);

        var options = new DbContextOptionsBuilder<MovieWatchlistDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new MovieWatchlistDbContext(options);
    }

    private static string GetConnectionString(string[] args)
    {
        // Priority: command-line args > environment variable > fallback
        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            return args[0];

        var env = Environment.GetEnvironmentVariable("ConnectionStrings__MovieWatchlist");
        if (!string.IsNullOrWhiteSpace(env))
            return env;

        // Fallback for local dev — user should replace this via secret.json or env var
        return "Server=(localdb)\\mssqllocaldb;Database=MovieWatchlist_Dev;Trusted_Connection=True;TrustServerCertificate=True;";
    }
}
