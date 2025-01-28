namespace LionFire.Reactive.Persistence;

public interface IObservableReaderWriter<TKey, TValue>
    : IObservableReader<TKey, TValue>
    , IObservableWriter<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
}
