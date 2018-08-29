namespace LionFire.Structures
{
    public interface IUpcastableWriteWrapper<in T> : IWriteWrapper<T>
    {
        IWriteWrapper<NewType> GetWriterapper<NewType>();
    }

}
