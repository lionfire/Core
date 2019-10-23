namespace LionFire.Persistence
{
#warning NEXT? IReadOnlyHandle IWriteOnlyHandle
    public interface IReadOnlyHandle<T> { } // TODO
    public interface IWriteOnlyHandle<T> { } // TODO

    public interface IDualHandle<T>
    {
        IReadOnlyHandle<T> LocalHandle { get; }
        IWriteOnlyHandle<T> RemoteHandle { get; }        
    }
}
