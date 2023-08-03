
namespace LionFire.Data.Async.Gets;

// REVIEW - Renamed to DisposableKeyed, move to LionFire.Structures namespace and maybe DLL?
// Is the Disposable part necessary for Resolvables?  It can act as a forceful signal from one part of an app to another that a handle/resolvable is dead and must not be used anymore.
// Perhaps this could trigger the cleanup of any events and unmanaged resources in complex derived classes such as ones that sync
public abstract class DisposableKeyed<TKey> : IDisposable, IKeyed<TKey>
     //where TKey : class
{
    protected DisposableKeyed() { }
    protected DisposableKeyed(TKey? key) { this.Key = key; }

    #region Input

    [SetOnce]
    public TKey? Key
    {
        get => isDisposed ? throw new ObjectDisposedException(nameof(DisposableKeyed<TKey>)) : key;
        set
        {
            //if (ReferenceEquals(key, value)) return;
            if (EqualityComparer<TKey>.Default.Equals(key, value)) return;
            if (!EqualityComparer<TKey>.Default.Equals(key, default)) throw new AlreadySetException();
            //if (key != default) throw new AlreadySetException();
            key = value;
        }
    }
    protected TKey? key;

    public virtual void Dispose()
    {
        // TODO: THREADSAFETY
        isDisposed = true;
        key = default;
    }
    protected bool isDisposed;

    #endregion
}
