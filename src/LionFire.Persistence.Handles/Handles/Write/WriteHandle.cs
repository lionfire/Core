using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public abstract class WriteHandle<TValue> : WriteHandle<IReference, TValue>, IWriteHandle<TValue>
        where TValue : class
    {

    }
}
