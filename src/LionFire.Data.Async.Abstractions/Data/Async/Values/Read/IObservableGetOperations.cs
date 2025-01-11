using System.Reactive.Linq;

namespace LionFire.Data.Async.Gets;


/// <summary>
/// 
/// </summary>
/// <remarks>
/// Also consider adding:
/// - IObservableGetState
/// </remarks>
/// <typeparam name="TValue"></typeparam>
public interface IObservableGetOperations<out TValue>
    //: IStatelessGetter<TValue>
    //: IObservableGetState // TODO: Add here? Or maybe it is too much of a burden and sometimes difficult to implement
{

    IObservable<ITask<IGetResult<TValue>>> GetOperations { get; }

   IObservable<IGetResult<TValue>> GetResults => GetOperations.SelectMany(t => Observable.FromAsync(async () => await t.ConfigureAwait(false)));

}
