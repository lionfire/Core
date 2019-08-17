using System;

namespace LionFire.Persistence
{
    [Obsolete("Use H<T>")]
    public interface IHandle<T> : IWriteHandle<T>, IReadHandle<T> { }
}
