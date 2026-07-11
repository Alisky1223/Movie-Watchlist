namespace SharedKernel.Domain.Services;

/// <summary>
/// Production implementation of <see cref="IClock"/> using real system time.
/// </summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
