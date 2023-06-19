using LionFire.Data.Async.Gets;
using LionFire.Structures;

namespace LionFire.Data.Sync.Gets;

public interface ILazilyGetsSync<out T> : ILazilyGets, IReadWrapper<T>
{
    ILazyGetResult<T> GetValue();
}

//public interface INotifyingLazilyResolves // Use Persistence instead?
//{
//    public event Action<ILazilyGets> Resolved;
//    public event Action<ILazilyGets> Discarded;
//}
