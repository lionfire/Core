using MorseCode.ITask;

namespace LionFire.Data.Gets;

public interface IObservableGetOperations<out TValue>
{
    
   IObservable<ITask<IGetResult<TValue>>> GetOperations { get; }
}
