using LionFire.Data.Async.Sets;

namespace LionFire.Persistence.Implementation;

public interface IHandleImpl<T> : IReadWriteHandleBase<T>, ISets, IDeletableImpl { }
