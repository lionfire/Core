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

        public IReadHandle<T> GetReadHandle<T>(IReference reference, T preresolvedValue = default) => throw new System.NotImplementedException();
        //IReadHandle<TValue> IReadHandleProvider<TReference>.GetReadHandle<TValue>(TReference reference) => throw new System.NotImplementedException();
        //IReadHandle<TValue> IReadHandleProvider.GetReadHandle<TValue>(IReference reference) => (reference is TReference tReference) ? GetReadHandle<TValue>(tReference) : null;

    }
}
