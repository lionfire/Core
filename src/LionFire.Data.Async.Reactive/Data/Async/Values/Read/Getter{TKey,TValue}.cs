
namespace LionFire.Data.Async;

public abstract class Getter<TKey, TValue>
    : GetterRxO<TValue>
    , IDisposable
    , IKeyed<TKey>
{
    #region Options

    public static GetsForKeyOptions DefaultGetsForKeyOptions { get; set; } = new();
    public virtual GetsForKeyOptions GetsForKeyOptions => DefaultGetsForKeyOptions;

    #endregion

    #region Key

    public TKey Key => isDisposed ? throw new ObjectDisposedException(null) : key;
    protected TKey key;

    public IAsyncObject? AsyncObjectKey => Key as IAsyncObject;

    #endregion

    #region Lifecycle

    protected Getter(TKey key, GetterOptions? options) : base(options)
    {
        this.key = key;
    }

    public virtual void Dispose()
    {
        isDisposed = true;
        var keyCopy = key;
        key = default;
        if (keyCopy is IDisposable d && GetsForKeyOptions.DisposeKey)
        {
            d.Dispose();
        }
    }
    protected bool isDisposed;

    #endregion
}

