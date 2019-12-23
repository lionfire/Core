using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.Persistence.Persisters
{
    public class PersisterHandleProvider<TReference> : PersisterHandleProviderBase<TReference>, IReadHandleProvider<TReference>, IReadWriteHandleProvider<TReference>
        where TReference : IReference
    {
        public PersisterHandleProvider(IPersisterProvider<TReference> persisterProvider) : base(persisterProvider)
        {
        }
    }
}
