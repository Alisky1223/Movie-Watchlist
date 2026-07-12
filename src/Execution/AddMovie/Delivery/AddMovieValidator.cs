using Execution.AddMovie.Domain.ValueObjects;
using Execution.Domain.ValueObjects;
using SharedKernel.Domain.ValueObjects;

namespace Execution.AddMovie.Delivery;

/// <summary>
/// Validates the add-movie request contract.
/// </summary>
public static class AddMovieValidator
{
    public static Result<AddMovieCommand> Validate(AddMovieRequest request)
    {
        var title = request.Title?.Trim();
        if (string.IsNullOrWhiteSpace(title) || title.Length is < 1 or > 200)
        {
            return Result<AddMovieCommand>.Failure(new MovieValidationError("Title must be between 1 and 200 characters."));
        }

        if (!MovieGenreParser.TryParse(request.Genre, out var genre))
        {
            return Result<AddMovieCommand>.Failure(new MovieValidationError("Genre must be one of the supported movie genres."));
        }

        return Result<AddMovieCommand>.Success(new AddMovieCommand(title, genre));
    }
}