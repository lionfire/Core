using LionFire.Referencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    //public abstract class ResolvableMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection> : AsyncMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection>
    //    //, IResolves<TUnderlyingCollectionItem>
    //    //where TUnderlyingCollection : IResolves<TUnderlyingCollectionItem>
    //    where TUnderlyingCollection : IEnumerable<TUnderlying>
    //{
    //    public ResolvableMapper(TUnderlyingCollection underlying) : base(underlying)
    //    {
    //    }

    //    //public ITask<IResolveResult<TUnderlyingCollectionItem>> Resolve() => UnderlyingHandle.Resolve();
    //}
    //public abstract class AsyncResolvableMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection> : AsyncMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection>
    //    //, IResolves<TUnderlyingCollectionItem>
    //    where TUnderlyingCollection : IResolves<TUnderlyingCollectionItem>, IEnumerable<TUnderlying>
    //{
    //    public AsyncResolvableMapper(TUnderlyingCollection underlying) : base(underlying)
    //    {
    //    }

    //    //public ITask<IResolveResult<TUnderlyingCollectionItem>> Resolve() => UnderlyingResolves.Resolve();
    //}

    public class ListingValues<TItem>
        : AsyncMapper<TItem,
            Listing<TItem>,
            IEnumerable<Listing<TItem>>,
            //IReadHandleBase<Metadata<IEnumerable<Listing<TItem>>>>,
            Metadata<IEnumerable<Listing<TItem>>>>
    {
        IReference reference;

        public ListingValues(IReadHandleBase<Metadata<IEnumerable<Listing<TItem>>>> listings) : base(listings)
        {
            reference = listings.Reference;
        }

        public override async Task<TItem> Map(Listing<TItem> underlying)
        {
            var result = await reference.GetChild(underlying.Name).GetReadHandle<TItem>().TryGetValue().ConfigureAwait(false);
            return result.Value;
        }
    }
}
