namespace SharedKernel.Domain.ValueObjects;

/// <summary>
/// Base type for all domain errors. Each error carries a stable code and a human-readable message.
/// </summary>
public abstract record Error(string Code, string Message)
{
    public static readonly Error None = new NoError();

    private sealed class NoError() : Error("None", "No error.");
}
