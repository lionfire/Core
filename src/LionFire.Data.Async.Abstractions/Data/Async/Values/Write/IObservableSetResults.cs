
namespace LionFire.Data.Async.Sets;

public interface IObservableSetResults<out TValue>
{
    IObservable<ISetResult<TValue>> SetResults { get; }
}
