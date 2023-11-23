using System.Reactive.Linq;

namespace LionFire.Data.Async.Gets;

public interface IObservableGetOperations<out TValue>
    //: IStatelessGetter<TValue>
{    
   IObservable<ITask<IGetResult<TValue>>> GetOperations { get; }

   IObservable<IGetResult<TValue>> GetResults => GetOperations.SelectMany(t => Observable.FromAsync(async () => await t.ConfigureAwait(false)));

}
