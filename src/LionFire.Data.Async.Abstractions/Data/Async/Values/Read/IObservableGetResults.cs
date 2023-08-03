
namespace LionFire.Data.Async.Gets;

public interface IObservableGetResults<out TValue>
{
    IObservable<IGetResult<TValue>> GetResults { get; }
}
