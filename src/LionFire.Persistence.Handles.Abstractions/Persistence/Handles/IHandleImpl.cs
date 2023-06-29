using LionFire.Data.Sets;

namespace LionFire.Persistence.Implementation;

public interface IHandleImpl<T> : IReadWriteHandleBase<T>, ISets, IDeletableImpl { }
