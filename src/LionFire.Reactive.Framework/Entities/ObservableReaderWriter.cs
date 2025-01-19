
using DynamicData.Binding;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LionFire.Reactive.Persistence;

public class ObservableReaderWriter<TKey, TValue> : IObservableReaderWriter<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{

    #region Relationships

    public IObservableReader<TKey, TValue> Read { get; }
    public IObservableWriter<TKey, TValue> Write { get; }

    #endregion

    #region Lifecycle

    public ObservableReaderWriter(IObservableReader<TKey, TValue> r, IObservableWriter<TKey, TValue> w)
    {
        Read = r;
        Write = w;
    }

    #endregion

}
public class InMemoryReaderWriterRx<TKey, TValue> : IObservableReaderWriter<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{

    public IObservableReader<TKey, TValue> Read => impl;
    public IObservableWriter<TKey, TValue> Write => impl;
    private InMemoryReaderWriterRxImpl<TKey, TValue> impl;

    public InMemoryReaderWriterRx(Func<TValue, TKey> keySelector)
    {
    }
}
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
        Items = KeyedItems.Connect().Transform(x => x.Value).AsObservableCache();
    }

    #region State

    public IObservableList<TKey> Keys => keys.ToObservableChangeSet().AsObservableList();
    private readonly ObservableCollection<TKey> keys = new();

    public IObservableCache<KeyValuePair<TKey, TValue>, TKey> KeyedItems => keyedItems;
    private SourceCache<KeyValuePair<TKey, TValue>, TKey> keyedItems = new(kvp => kvp.Key);
    public IObservableCache<TValue, TKey> Items { get; }

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


