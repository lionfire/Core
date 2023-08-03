
namespace LionFire.Data.Async.Sets;

public interface IObservableSetOperations<out TValue>
{

    IObservable<ITask<ISetResult<TValue>>> SetOperations { get; }
}
