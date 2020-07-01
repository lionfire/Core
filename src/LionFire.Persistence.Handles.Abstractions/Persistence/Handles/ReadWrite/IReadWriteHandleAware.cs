namespace LionFire.Persistence
{
    public interface IReadWriteHandleAware<T>
    {
        IReadWriteHandle<T> ReadWriteHandle { get; set; }
    }
}
