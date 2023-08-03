
namespace LionFire.Data.Async.Gets;

public interface IObservableGetOperations<out TValue>
    //: IStatelessGetter<TValue>
{    
   IObservable<ITask<IGetResult<TValue>>> GetOperations { get; }
}
