﻿using LionFire.Referencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    //public abstract class ResolvableMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection> : AsyncMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection>
    //    //, IGets<TUnderlyingCollectionItem>
    //    //where TUnderlyingCollection : IGets<TUnderlyingCollectionItem>
    //    where TUnderlyingCollection : IEnumerable<TUnderlying>
    //{
    //    public ResolvableMapper(TUnderlyingCollection underlying) : base(underlying)
    //    {
    //    }

    //    //public ITask<IGetResult<TUnderlyingCollectionItem>> Resolve() => UnderlyingHandle.Resolve();
    //}
    //public abstract class AsyncResolvableMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection> : AsyncMapper<TItem, TUnderlying, TUnderlyingCollection, TResolvedUnderlyingCollection>
    //    //, IGets<TUnderlyingCollectionItem>
    //    where TUnderlyingCollection : IGets<TUnderlyingCollectionItem>, IEnumerable<TUnderlying>
    //{
    //    public AsyncResolvableMapper(TUnderlyingCollection underlying) : base(underlying)
    //    {
    //    }

    //    //public ITask<IGetResult<TUnderlyingCollectionItem>> Resolve() => UnderlyingResolves.Resolve();
    //}

    public class ListingValues<TItem>
        : AsyncMapper<TItem,
            IListing<TItem>,
            IEnumerable<IListing<TItem>>,
            //IReadHandleBase<Metadata<IEnumerable<IListing<TItem>>>>,
            Metadata<IEnumerable<IListing<TItem>>>>
    {
        IReference reference;

        public ListingValues(IReadHandleBase<Metadata<IEnumerable<IListing<TItem>>>> listings) : base(listings)
        {
            reference = listings.Reference;
        }

        public override async Task<TItem> Map(IListing<TItem> underlying)
        {
            var result = await reference.GetChild(underlying.Name).GetReadHandle<TItem>().TryGetValue().ConfigureAwait(false);
            return result.Value;
        }
    }
}
