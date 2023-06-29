using LionFire.Data;

namespace LionFire.Persistence.Persisters.Vos;

public interface IVosRetrieveResult<out T> : IGetResult<T>
{
    public IReadHandleBase<T> ReadHandle { get; }
}
