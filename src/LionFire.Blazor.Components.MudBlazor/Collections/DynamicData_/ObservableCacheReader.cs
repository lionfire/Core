using DynamicData;
using DynamicData.Kernel;
using LionFire.Reactive.Persistence;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace LionFire.Blazor.Components;

public class ObservableCacheReader<TKey, TValue> : IObservableReader<TKey, TValue>, IDisposable
    where TKey : notnull
    where TValue : notnull
{
    private readonly IObservableCache<TValue, TKey> source;

    public ObservableCacheReader(IObservableCache<TValue, TKey> source)
    {
        this.source = source;
    }

    private IObservableCache<TKey, TKey>? keys;
    public IObservableCache<TKey, TKey> Keys => keys ??= source.Connect().Transform((v, k) => k).AsObservableCache();

    private IObservableCache<Optional<TValue>, TKey>? values;
    public IObservableCache<Optional<TValue>, TKey> Values => values ??= source.Connect().Transform(v => (Optional<TValue>)v).AsObservableCache();

    public IObservableCache<Optional<TValue>, TKey> ObservableCache => Values;

    public IDisposable ListenAllKeys() => Disposable.Empty;

    public ValueTask<IDisposable> ListenAllValues() => new(Disposable.Empty);

    public IObservable<TValue?> GetValueObservable(TKey key)
    {
        return source.Watch(key).Select(c => c.Reason == ChangeReason.Add || c.Reason == ChangeReason.Update ? c.Current : default(TValue?));
    }

    public IObservable<TValue?>? GetValueObservableIfExists(TKey key)
    {
        return GetValueObservable(key);
    }

    public void Dispose()
    {
        (keys as IDisposable)?.Dispose();
        (values as IDisposable)?.Dispose();
    }
}
