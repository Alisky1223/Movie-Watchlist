using SharedKernel.Domain.ValueObjects;

namespace Execution.Domain.ValueObjects;

/// <summary>
/// Typed domain errors for Movie aggregate.
/// </summary>
public sealed record MovieAlreadyExistsError(string Title)
    : Error("MovieAlreadyExists", $"A movie with the title '{Title}' already exists.");

public sealed record MovieNotFoundError(Guid Id)
    : Error("MovieNotFound", $"Movie with Id '{Id}' was not found.");
