
using DynamicData.Binding;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LionFire.Reactive.Persistence;

internal class InMemoryReaderWriterRxImpl<TKey, TValue>
    : IObservableReader<TKey, TValue>
    , IObservableWriter<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    public InMemoryReaderWriterRxImpl()
    {
        //KeyedItems = keys.ToObservableChangeSet()
        //    .Transform(key => new KeyValuePair<TKey, TValue>(key, ReadFromFile(GetFilePath(key))!))
        //    .AddKey(kvp => kvp.Key)
        //    //.Flatten()
        //    //.Where(change => change.Current.Value != null)
        //    .AsObservableCache()
        //    ;
        ObservableCache = KeyedItems.Connect().Transform(x => x.Value).AsObservableCache();
    }

    #region State

    public IObservableCache<TKey, TKey> Keys => keys.AsObservableCache();
    private readonly SourceCache<TKey, TKey> keys = new(k => k);

    public IObservableCache<KeyValuePair<TKey, TValue>, TKey> KeyedItems => keyedItems;
    private SourceCache<KeyValuePair<TKey, TValue>, TKey> keyedItems = new(kvp => kvp.Key);
    public IObservableCache<TValue, TKey> ObservableCache { get; }

    #endregion

    #region Write

    public IDisposable Synchronize(IObservable<TValue> source, TKey key, WritingOptions? options = null)
    {
        return source
            //.Throttle(options?.DebounceDelay ?? TimeSpan.FromSeconds(1))
            .Subscribe(value => keyedItems.Edit(u => u.AddOrUpdate(new KeyValuePair<TKey, TValue>(key, value))));
    }

    public IDisposable Synchronize(IReactiveNotifyPropertyChanged<IReactiveObject> source, TKey key, WritingOptions? options = null)
    {
        if (source is TValue x)
        {
            return source.Changed
                //.Throttle(options?.DebounceDelay ?? TimeSpan.FromSeconds(1))
                .Subscribe(_ => keyedItems.Edit(u => u.AddOrUpdate(new KeyValuePair<TKey, TValue>(key, x))));
        }
        else
        {
            throw new ArgumentException($"{nameof(source)} must be of type {typeof(TValue).Name}");
        }
    }

    public ValueTask Write(TKey key, TValue value)
    {
        keyedItems.Edit(u => u.AddOrUpdate(new KeyValuePair<TKey, TValue>(key, value)));
        return ValueTask.CompletedTask;
    }
    public ValueTask<bool> Remove(TKey key)
    {
        var result = keyedItems.Lookup(key).HasValue; // Result is not thread safe
        keyedItems.Edit(u => u.RemoveKey(key));
        return ValueTask.FromResult(true);
    }

    public IObservable<TValue?> Listen(TKey key)
    {
        throw new NotImplementedException();
    }

    #endregion

}


