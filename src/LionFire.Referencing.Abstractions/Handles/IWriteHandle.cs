using LionFire.Persistence;

namespace LionFire
{
    public interface IWriteHandle<in T> : ISaveable
    {
        void SetObject(T obj);
    }
}

