using System.Threading.Tasks.Sources;

namespace LionFire.Data.Gets;

public interface IObservableGetResults<out TValue>
{
    IObservable<IGetResult<TValue>> GetResults { get; }
}
