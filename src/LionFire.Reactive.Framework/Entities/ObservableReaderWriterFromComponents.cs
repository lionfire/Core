using DynamicData.Kernel;
using ReactiveUI;

namespace LionFire.Reactive.Persistence;

public class ObservableReaderWriterFromComponents<TKey, TValue> : IObservableReaderWriterComponents<TKey, TValue>
    , IObservableReaderWriter<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    #region Relationships

    public IObservableReader<TKey, TValue> Read { get; }
    public IObservableWriter<TKey, TValue> Write { get; }

    #endregion

    #region Lifecycle

    public ObservableReaderWriterFromComponents(IObservableReader<TKey, TValue> r, IObservableWriter<TKey, TValue> w)
    {
        Read = r;
        Write = w;
    }
    #endregion

    #region Reader

    public IObservableCache<TKey, TKey> Keys => Read.Keys;

    public IObservableCache<Optional<TValue>, TKey> Values => Read.Values;

    //public IObservable<TValue?> GetListener(TKey key) => Read.GetListener(key);
    public IDisposable ListenAll() => Read.ListenAll();

    public IObservable<TValue?> GetValueObservableIfExists(TKey key) => Read.GetValueObservableIfExists(key);

    public IObservable<TValue?> GetValueObservable(TKey key) => Read.GetValueObservable(key);

    #endregion

    #region Writer

    ValueTask IObservableWriter<TKey, TValue>.Write(TKey key, TValue value)
    {
        return Write.Write(key, value);
    }

    public ValueTask<bool> Remove(TKey key)
    {
        return Write.Remove(key);
    }

    public IDisposable Synchronize(IObservable<TValue> source, TKey key, WritingOptions? options = null)
    {
        return Write.Synchronize(source, key, options);
    }

    public IDisposable Synchronize(IReactiveNotifyPropertyChanged<IReactiveObject> source, TKey key, WritingOptions? options = null)
    {
        return Write.Synchronize(source, key, options);
    }


    #endregion

}

