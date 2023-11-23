#nullable enable
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    /// <summary>
    /// Attempts to deserializes every child (SLOW).  Consider using ConventionListProvider instead.
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    public class DefaultListProvider<TReference> : IListProvider<TReference>
        where TReference : IReference
    {
#nullable disable
        public async Task<IEnumerable<IListing<TChildValue>>> List<TChildValue>(IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null)
           => (await Task.WhenAll(
              (await persister.List<TChildValue>(referencable, filter).ConfigureAwait(false)).ThrowIfUnsuccessful().Value
                .Select(async listing => new { Listing = listing, hasValue = (await referencable.Reference.GetChild(listing.Name).GetReadHandle<TChildValue>().Get()).HasValue })
                ).ConfigureAwait(false))
                //.Where(t => t.hasValue).Select(t => new Metadata<Listing>(t.Listing))
                .Where(t => t.hasValue).Select(t => t.Listing)
                ;
#nullable enable
      }
}

