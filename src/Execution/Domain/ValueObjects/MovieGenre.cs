namespace Execution.Domain.ValueObjects;

/// <summary>
/// Supported movie genres. PascalCase singular per naming rules.
/// </summary>
public enum MovieGenre
{
    Action,
    Comedy,
    Drama,
    Horror,
    SciFi,
    Romance,
    Thriller,
    Animation,
    Documentary,
    Other
}

/// <summary>
/// String parsing helper for movie genres used by HTTP DTOs.
/// </summary>
public static class MovieGenreParser
{
    public static bool TryParse(string? value, out MovieGenre genre)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            genre = default;
            return false;
        }

        var normalized = value.Trim().Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
        if (Enum.TryParse<MovieGenre>(normalized, true, out genre))
        {
            return true;
        }

        genre = default;
        return false;
    }
}
