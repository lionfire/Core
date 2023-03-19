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
    //    //RCollectionBase<TReference, IReadHandle<Metadata<IEnumerable<IListing<T>>>>, TItem>, HC<TReference,string>
    //    , IHasUnderlyingReadHandle<Metadata<IEnumerable<IListing<TItem>>>>
    //{
    //    public IReadHandleBase<Metadata<IEnumerable<IListing<TItem>>>> UnderlyingHandle => listings;
    //    IReadHandleBase<Metadata<IEnumerable<IListing<TItem>>>> listings;

    //    public IAsyncEnumerator<TItem> GetAsyncEnumerator(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    //    public ReadHandles(IReadHandleBase<Metadata<IEnumerable<IListing<TItem>>>> listings)
    //    {
    //        this.listings = listings;
    //    }

    //}

}
