namespace Pingmint;

public class NoOpDisposable : IDisposable
{
    public static readonly NoOpDisposable Instance = new NoOpDisposable();

    private NoOpDisposable() { }

    void IDisposable.Dispose() { }
}

public class NoOpAsyncDisposable : IAsyncDisposable
{
    public static readonly NoOpAsyncDisposable Instance = new NoOpAsyncDisposable();

    private NoOpAsyncDisposable() { }

    ValueTask IAsyncDisposable.DisposeAsync() => ValueTask.CompletedTask;
}
