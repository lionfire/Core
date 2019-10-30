using LionFire.Persistence;
using LionFire.Resolves;

namespace LionFire
{
    public interface IWriteHandle<in T> : IPuts, IDeletable
    {
        void SetObject(T obj);
    }
}

