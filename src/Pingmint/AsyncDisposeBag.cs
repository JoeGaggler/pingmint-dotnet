
namespace Pingmint;

/// <summary>
/// A collection of <see cref="IAsyncDisposable" /> objects that have a shared lifetime.
/// </summary>
public class AsyncDisposeBag : IAsyncDisposable
{
    private readonly Stack<IAsyncDisposable> items = new();

    /// <summary>
    /// Adds an IAsyncDisposable object to be disposed.
    /// </summary>
    /// <param name="disposable"><see cref="IAsyncDisposable" /> instance.</param>
    public void Add(IAsyncDisposable? disposable)
    {
        if (disposable is null) { return; }
        items.Push(disposable);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        while (items.Count > 0)
        {
            var item = items.Pop();
            await item.DisposeAsync();
        }
    }
}
