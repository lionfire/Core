namespace LionFire.Persistence
{
    using LionFire.Persistence.Handles;
    using LionFire.Resolves;
    using LionFire.Structures;

    /// <summary>
    /// IReadWriteHandleEx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Wx<T> : IReadHandleEx<T>, W<T>
    {
    }

    /// <summary>
    /// IReadWriteHandle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface W<T> : IHandleBase, RH<T>, IWrapper<T>, IPuts, IDeletable, IWriteHandle<T> { } // Rename to W?  And write-only: WO

}
