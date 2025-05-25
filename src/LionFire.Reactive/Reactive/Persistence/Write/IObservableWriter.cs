using LionFire.Reactive.Persistence;
using LionFire.Data.Async.Sets;
using ReactiveUI;

namespace LionFire.Reactive.Persistence;


public interface IObservableWriter<TKey, TValue> : IDisposable
//: ISetter<TValue> // Need non-targeted variant on ISetter that accepts a key parameter.
{
    ValueTask Write(TKey key, TValue value); // TODO: ISetResult
    ValueTask<bool> Remove(TKey key);

    public IObservable<WriteOperation<TKey, TValue>> WriteOperations { get; }

    IDisposable Synchronize(IObservable<TValue> source, TKey key, WritingOptions? options = null);
    IDisposable Synchronize(IReactiveNotifyPropertyChanged<IReactiveObject> source, TKey key, WritingOptions? options = null);
}
