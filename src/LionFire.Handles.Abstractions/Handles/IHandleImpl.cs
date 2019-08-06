namespace LionFire.Persistence.Implementation
{
    public interface IHandleImpl<T> : H<T>, ICommitableImpl, IDeletableImpl { }

}
