using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IPersisterHandle<TReference>
        where TReference : IReference
    {
        IPersister<TReference> Persister { get; }
    }
}
