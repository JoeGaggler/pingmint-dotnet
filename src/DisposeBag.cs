namespace Pingmint;

/// <summary>
/// A collection of IDisposable objects that have a shared lifetime.
/// </summary>
public class DisposeBag : IDisposable
{
    private readonly Stack<IDisposable> items = new();

    public void Add(IDisposable? disposable)
    {
        if (disposable is null) { return; }
        items.Push(disposable);
    }


    public void Dispose()
    {
        while (items.TryPop(out var item))
        {
            item.Dispose();
        }
    }
}
