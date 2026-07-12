namespace SharedKernel.Domain.Services;

/// <summary>
/// Abstraction for time. Enables testability and deterministic time in tests.
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
