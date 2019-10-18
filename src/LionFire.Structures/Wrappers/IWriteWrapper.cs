namespace LionFire.Structures
{
    public interface IWriteWrapper<in T>
    {
        T Object { set; } // RENAME to Value
    }
}
