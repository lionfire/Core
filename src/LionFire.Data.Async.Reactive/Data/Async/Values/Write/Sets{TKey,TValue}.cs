
namespace LionFire.Data.Async.Sets;

public class SetsForKeyOptions
{
    public bool DisposeKey { get; set; } = true;
}


public abstract class Sets<TKey, TValue>
	: Sets<TValue>
{
    #region Options

    public static SetsForKeyOptions DefaultSetsForKeyOptions { get; set; } = new();
    public virtual SetsForKeyOptions SetsForKeyOptions => DefaultSetsForKeyOptions;

    #endregion

    #region Key

    public TKey Key => key ?? throw new ObjectDisposedException(null);
    protected TKey? key;

    public IAsyncObject? AsyncObjectKey => Key as IAsyncObject;

    #endregion

    #region Lifecycle

    public Sets() : this(null) { }

    public Sets(SetterOptions? options) : base(options)
    {
    }
        
    public virtual void Dispose()
    {
        isDisposed = true;
        var keyCopy = key;
        key = default;
        if (keyCopy is IDisposable d && SetsForKeyOptions.DisposeKey)
        {
            d.Dispose();
        }
    }
    protected bool isDisposed;

    #endregion
}
