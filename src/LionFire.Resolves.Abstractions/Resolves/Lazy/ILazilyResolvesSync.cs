using LionFire.Structures;

namespace LionFire.Data.Async.Gets
{
    public interface ILazilyResolvesSync<out T> : ILazilyResolves, IReadWrapper<T>
    {
        ILazyResolveResult<T> GetValue();
    }

    //public interface INotifyingLazilyResolves // Use Persistence instead?
    //{
    //    public event Action<ILazilyResolves> Resolved;
    //    public event Action<ILazilyResolves> Discarded;
    //}
}
