using DynamicData.Kernel;
using LionFire.Reactive;

namespace LionFire.IO.Reactive;

public abstract class ObservableReaderBase<TKey, TValue> //: IObservableReader<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    public ObservableReaderBase()
    {
        Values = values.Connect().Transform(x => x.optional).AsObservableCache();
    }

    #region State

    protected IObservableCache<(TKey key, Optional<TValue> optional), TKey> KeyedValues => values.AsObservableCache();
    protected SourceCache<(TKey key, Optional<TValue> optional), TKey> values = new(x => x.key);

    #region Derived

    public IObservableCache<Optional<TValue>, TKey> Values { get; }
    //    valuesWithSubscribeEvents ??=
    //        values
    //            .ConnectOnDemand(v => v)
    //            .PublishRefCountWithEvents(() => EnableKeysWatcher(), () => DisableKeysWatcher())
    //            .AsObservableCache();
    //private IObservableCache<Optional<TValue>, TKey>? valuesWithSubscribeEvents;

    public abstract IObservableCache<Optional<TValue>, TKey> ObservableCache { get; }

    #endregion

    #endregion
}
