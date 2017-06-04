using LionFire.Persistence;

namespace LionFire
{
    public interface IWriteHandle<in T> : ISaveable
    {
        T Object { set; }
    }
}

