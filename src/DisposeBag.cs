namespace Pingmint;

/// <summary>
/// A collection of IDisposable objects that have a shared lifetime.
/// </summary>
public class DisposeBag : IDisposable
{
    private readonly Stack<IDisposable> items = new();

    /// <summary>
    /// Adds an IDisposable object to be disposed.
    /// </summary>
    /// <param name="disposable"><see cref="IDisposable" /> instance.</param>
    public void Add(IDisposable? disposable)
    {
        if (disposable is null) { return; }
        items.Push(disposable);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        while (items.Count > 0)
        {
            var item = items.Pop();
            item.Dispose();
        }
    }
}
