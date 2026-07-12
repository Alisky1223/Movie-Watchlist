using Execution.Domain.ValueObjects;
using Execution.UpdateMovie.Domain.ValueObjects;
using SharedKernel.Domain.ValueObjects;

namespace Execution.UpdateMovie.Delivery;

/// <summary>
/// Validates the update-movie request contract.
/// </summary>
public static class UpdateMovieValidator
{
    public static Result<UpdateMovieCommand> Validate(UpdateMovieRequest request)
    {
        if (request.Title is not null)
        {
            var title = request.Title.Trim();
            if (string.IsNullOrWhiteSpace(title) || title.Length is < 1 or > 200)
            {
                return Result<UpdateMovieCommand>.Failure(new MovieValidationError("Title must be between 1 and 200 characters."));
            }
        }

        if (request.Genre is not null && !MovieGenreParser.TryParse(request.Genre, out _))
        {
            return Result<UpdateMovieCommand>.Failure(new MovieValidationError("Genre must be one of the supported movie genres."));
        }

        return Result<UpdateMovieCommand>.Success(new UpdateMovieCommand(Guid.Empty, request.Title, request.Genre));
    }
}