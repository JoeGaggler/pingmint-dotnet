namespace Pingmint;

/// <summary>
/// Emulates a `defer` statement.
/// Syntax sugar for defining a `finally` block before the `try` block.
/// </summary>
/// <remarks>
/// Hopefully, this will be added to the language in the future.
/// </remarks>
public class Defer(Action action) : IDisposable
{
    private readonly Action action = action;

    void IDisposable.Dispose()
    {
        action();
    }
}
