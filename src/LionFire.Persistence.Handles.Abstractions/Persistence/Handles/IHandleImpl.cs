namespace LionFire.Persistence.Implementation
{
    public interface IHandleImpl<T> : IReadWriteHandleBase<T>, ICommitableImpl, IDeletableImpl { }

}
