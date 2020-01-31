using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System.Collections.Concurrent;

namespace LionFire.Persistence.Persisters
{
    public class SharingHandleProviderBase<TReference, THandle>
        where TReference : IReference
    {

        protected ConcurrentDictionary<TReference, THandle> handles = new ConcurrentDictionary<TReference, THandle>();

    }

    public class SharingHandleProvider<TReference> : SharingHandleProviderBase<TReference, object>, IReadHandleProvider<TReference>
        where TReference : IReference
    {
        public IReadHandleProvider<TReference> InnerReadHandleProvider { get; }

        public IReadHandle<T> GetReadHandle<T>(TReference reference)
        {
            throw new System.NotImplementedException();
        }

        public IReadHandle<T> GetReadHandle<T>(IReference reference) => reference is TReference tReference ? GetReadHandle<T>(tReference) : null;

    }
}
