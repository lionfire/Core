
namespace LionFire.Data.Sets;

public abstract class AsyncSets<TKey, TValue>
	: AsyncSets<TValue>
{
    #region Key

    public TKey Key => key ?? throw new ObjectDisposedException(null);
    protected TKey? key;

    public IAsyncObject? AsyncObjectKey => Key as IAsyncObject;

    #endregion

    #region Lifecycle

    public AsyncSets() : this(null) { }

    public AsyncSets(AsyncSetOptions? options) : base(options)
    {
    }
        
    public virtual void Dispose()
    {
        isDisposed = true;
        var keyCopy = key;
        key = default;
        if (keyCopy is IDisposable d && Options.DisposeKey) // ENH: Offload this to implementors
        {
            d.Dispose();
        }
    }
    protected bool isDisposed;

    #endregion
}
