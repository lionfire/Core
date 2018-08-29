namespace LionFire.Structures
{
    public interface IUpcastableReadWrapper<out T> : IReadWrapper<T>
    {        
        IReadWrapper<NewType> GetReadWrapper<NewType>();
    }

}
