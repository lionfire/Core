using LionFire.Structures;

namespace LionFire.Resolves
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
