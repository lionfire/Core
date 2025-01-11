
namespace LionFire.Data.Async.Sets;


/// <summary>
/// 
/// </summary>
/// <remarks>
/// Also consider adding:
/// - IObservableSetState
/// </remarks>
/// <typeparam name="TValue"></typeparam>
public interface IObservableSetOperations<out TValue>
    //: IObservableSetState
{

    IObservable<ISetOperation<TValue>> SetOperations { get; }
}
