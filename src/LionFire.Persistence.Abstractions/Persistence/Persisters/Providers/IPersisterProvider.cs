#nullable enable

using LionFire.Referencing;

namespace LionFire.Persistence.Persisters
{
    public interface IPersisterProvider<TReference>
        where TReference : IReference
        //where TPersister : IPersister<TReference>
    {
        bool HasDefaultPersister { get; }
        IPersister<TReference> GetPersister(string? name = null);
    }
}
