
namespace LionFire.Data.Sets;

public interface IObservableSetOperations<out TValue>
{

    IObservable<ITask<IGetResult<TValue>>> SetOperations { get; }
}
