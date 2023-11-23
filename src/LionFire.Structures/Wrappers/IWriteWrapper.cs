namespace LionFire.Structures
{
    public interface IWriteWrapper<in T>
    {
        T Value { set; }
    }
}
