namespace LionFire.Persistence.Persisters.Vos
{
    public interface IVosRetrieveResult<out T> : IRetrieveResult<T>
    {
        public IReadHandleBase<T> ReadHandle { get; }
    }
}
