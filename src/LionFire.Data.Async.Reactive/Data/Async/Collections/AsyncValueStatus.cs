using ReactiveUI.SourceGenerators;
using ReactiveAttribute = ReactiveUI.SourceGenerators.ReactiveAttribute;

namespace LionFire.Data.Collections;

public partial class AsyncValueStatus<TKey, TValue> : ReactiveObject
    where TKey : notnull
    where TValue : notnull
{
    public TKey Key { get; }

    public AsyncValueStatus(TKey key)
    {
        Key = key;
    }

    [Reactive]
    private AsyncValueState _state;
    [Reactive]
    private TValue? _value;
}
