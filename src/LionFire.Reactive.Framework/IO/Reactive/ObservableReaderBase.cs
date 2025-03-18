using DynamicData.Kernel;

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

    public IObservableCache<Optional<TValue>, TKey> Values { get; }
    protected IObservableCache<(TKey key, Optional<TValue> optional), TKey> KeyedValues => values.AsObservableCache();
    protected SourceCache<(TKey key, Optional<TValue> optional), TKey> values = new(x => x.key);

    #endregion
}
