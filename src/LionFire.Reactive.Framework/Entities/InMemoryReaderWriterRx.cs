namespace LionFire.Reactive.Persistence;

public class InMemoryReaderWriterRx<TKey, TValue> : IObservableReaderWriterComponents<TKey, TValue>
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


