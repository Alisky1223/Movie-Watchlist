namespace SharedKernel.Domain.ValueObjects;

/// <summary>
/// Result type for predictable domain failures. Carries a value on success or an <see cref="Error"/> on failure.
/// Never throws for expected domain errors.
/// </summary>
public sealed record Result<T>
{
    public T? Value { get; }
    public Error Error { get; }
    public bool IsSuccess { get; }

    private Result(T value)
    {
        Value = value;
        Error = Error.None;
        IsSuccess = true;
    }

    private Result(Error error)
    {
        Value = default;
        Error = error;
        IsSuccess = false;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Error error) => new(error);
}

/// <summary>
/// Non-generic result for operations that do not return a value.
/// </summary>
public sealed record Result
{
    public Error Error { get; }
    public bool IsSuccess { get; }

    private Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);
}
