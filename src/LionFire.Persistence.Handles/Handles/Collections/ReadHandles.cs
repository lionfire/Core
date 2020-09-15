using LionFire.ExtensionMethods.Poco.Resolvables;
using LionFire.Resolvables;
using System.Collections;
using System.Net.Http.Headers;

namespace LionFire.Persistence.Handles
{
    public interface IHasUnderlyingReadHandle<T>
    {
        IReadHandleBase<T> UnderlyingHandle { get; }
    }

    //public class ReadHandles<TItem> : IAsyncEnumerable<TItem>
    //    //RCollectionBase<TReference, IReadHandle<Metadata<IEnumerable<Listing<T>>>>, TItem>, HC<TReference,string>
    //    , IHasUnderlyingReadHandle<Metadata<IEnumerable<Listing<TItem>>>>
    //{
    //    public IReadHandleBase<Metadata<IEnumerable<Listing<TItem>>>> UnderlyingHandle => listings;
    //    IReadHandleBase<Metadata<IEnumerable<Listing<TItem>>>> listings;

    //    public IAsyncEnumerator<TItem> GetAsyncEnumerator(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    //    public ReadHandles(IReadHandleBase<Metadata<IEnumerable<Listing<TItem>>>> listings)
    //    {
    //        this.listings = listings;
    //    }

    //}

}
