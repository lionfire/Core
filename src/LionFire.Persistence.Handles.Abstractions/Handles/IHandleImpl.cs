namespace LionFire.Persistence.Implementation
{
    public interface IHandleImpl<T> : W<T>, ICommitableImpl, IDeletableImpl { }

}
