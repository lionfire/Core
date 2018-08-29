namespace LionFire.Structures
{
    public interface IWriteWrapper<in T> //: IReadWrapper<T>
    {
        T Object { set; }
    }
}
