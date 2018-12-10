using LionFire.Persistence;

namespace LionFire
{
    public interface IWriteHandle<in T> : ICommitable, IDeletable
    {
        void SetObject(T obj);
    }
}

