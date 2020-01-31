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

        IReadHandle<T> IReadHandleProvider<TReference>.GetReadHandle<T>(TReference reference) => throw new System.NotImplementedException();
        IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference) => throw new System.NotImplementedException();
    }
}
