namespace LionFire.Persistence
{
    public interface IReadHandleAware<T>
    {
        IReadHandle<T> ReadHandle { get; set; }
    }
}
