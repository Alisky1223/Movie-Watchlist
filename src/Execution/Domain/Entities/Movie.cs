using SharedKernel.Domain.Services;
using Execution.Domain.ValueObjects;

namespace Execution.Domain.Entities;

/// <summary>
/// Movie entity — global aggregate. Title must be unique (case-insensitive, enforced by DB unique index + application check).
/// </summary>
public sealed class Movie
{
    public Guid Id { get; private init; }
    public string Title { get; private set; }
    public MovieGenre Genre { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }

    private Movie() { } // EF Core

    private Movie(Guid id, string title, MovieGenre genre, DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        Genre = genre;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Factory: creates a new Movie with generated Id and timestamp from <see cref="IClock"/>.
    /// </summary>
    public static Movie Create(string title, MovieGenre genre, IClock clock)
    {
        var trimmedTitle = title.Trim();
        ArgumentException.ThrowIfNullOrWhiteSpace(trimmedTitle);

        return new Movie(
            id: Guid.NewGuid(),
            title: trimmedTitle,
            genre: genre,
            createdAt: clock.UtcNow);
    }

    /// <summary>
    /// Applies partial changes from update command. CreatedAt is immutable.
    /// </summary>
    public void ApplyChanges(string? title, MovieGenre? genre)
    {
        if (title is not null)
        {
            var trimmed = title.Trim();
            ArgumentException.ThrowIfNullOrWhiteSpace(trimmed);
            Title = trimmed;
        }

        if (genre is not null)
        {
            Genre = genre.Value;
        }
    }
}
