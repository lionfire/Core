﻿#nullable enable
using LionFire.Referencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public static class IListProviderExtensions
    {
#if !NETSTANDARD_2_1
        public static Task<IEnumerable<IListing<TChildValue>>> List<TChildValue, TReference>(this IListProvider<TReference> listProvider, IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null)
            where TReference : IReference
           => listProvider.List<TChildValue>( persister, referencable, filter);
#endif
    }
}
