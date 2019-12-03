using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public interface IPersisterHandle<TReference, TPersister> : IPersisterHandle<TReference>
        where TReference : IReference
        where TPersister : IPersister<TReference>
    {
        new TPersister Persister { get; }
    }

    

}
