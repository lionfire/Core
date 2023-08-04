
namespace LionFire.Data.Async.Sets;

public interface IObservableSetOperations<out TValue>
{

    IObservable<ISetOperation<TValue>> SetOperations { get; }
}
