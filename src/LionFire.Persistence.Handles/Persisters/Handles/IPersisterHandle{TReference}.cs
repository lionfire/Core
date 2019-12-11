using LionFire.Referencing;

namespace LionFire.Persistence.Persisters
{
    public interface IPersisterHandle<TReference>
        where TReference : IReference
    {
        IPersister<TReference> Persister { get; }
    }
}
