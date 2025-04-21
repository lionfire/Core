
using DynamicData.Binding;
using DynamicData.Kernel;
using LionFire.IO.Reactive;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LionFire.Reactive.Persistence;

internal class InMemoryReaderWriterRxImpl<TKey, TValue>
    : ObservableReaderBase<TKey, TValue>
    , IObservableReader<TKey, TValue>
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
    }

    #region State

    public IObservableCache<TKey, TKey> Keys => keys.AsObservableCache();
    private readonly SourceCache<TKey, TKey> keys = new(k => k);


    #endregion
    public override IObservableCache<Optional<TValue>, TKey> ObservableCache => Values;

    #region Write

    public IDisposable Synchronize(IObservable<TValue> source, TKey key, WritingOptions? options = null)
    {
        return source
            //.Throttle(options?.DebounceDelay ?? TimeSpan.FromSeconds(1))
            .Subscribe(value => values.Edit(u => u.AddOrUpdate((key, value))));
    }

    public IDisposable Synchronize(IReactiveNotifyPropertyChanged<IReactiveObject> source, TKey key, WritingOptions? options = null)
    {
        if (source is TValue x)
        {
            return source.Changed
                //.Throttle(options?.DebounceDelay ?? TimeSpan.FromSeconds(1))
                .Subscribe(_ => values.Edit(u => u.AddOrUpdate((key, x))));
        }
        else
        {
            throw new ArgumentException($"{nameof(source)} must be of type {typeof(TValue).Name}");
        }
    }

    public ValueTask Write(TKey key, TValue value)
    {
        values.Edit(u => u.AddOrUpdate((key, value)));
        return ValueTask.CompletedTask;
    }
    public ValueTask<bool> Remove(TKey key)
    {
        var result = values.Lookup(key).HasValue; // Result is not thread safe
        values.Edit(u => u.RemoveKey(key));
        return ValueTask.FromResult(true);
    }

    public IObservable<TValue?> GetValueObservableIfExists(TKey key)
    {
        throw new NotImplementedException();
    }
    public IDisposable ListenAllKeys() => throw new NotImplementedException();
    public ValueTask<IDisposable> ListenAllValues() => throw new NotImplementedException();
    //public ValueTask<Optional<TValue>> TryGetValue(TKey key);
    public IObservable<TValue?> GetValueObservable(TKey key)
    {
        throw new NotImplementedException();
    }

    #endregion

}


