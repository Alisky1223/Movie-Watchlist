using Execution.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence;

/// <summary>
/// DI registration extension for Persistence infrastructure.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddMovieWatchlistPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<MovieWatchlistDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IMovieRepository, MovieRepository>();

        return services;
    }
}
