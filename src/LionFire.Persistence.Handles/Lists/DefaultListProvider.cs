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
        public async Task<IEnumerable<string>> List<TChildValue>(IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null)
           => (await Task.WhenAll(
               (await persister.List(referencable, filter).ConfigureAwait(false)).ThrowIfUnsuccessful<IRetrieveResult<IEnumerable<string>>>().Value
                .Select(async n => new { name = n, hasValue = (await referencable.Reference.GetChild(n).GetReadHandle<TChildValue>().Resolve()).HasValue })
                ).ConfigureAwait(false))
                .Where(t => t.hasValue).Select(t => t.name)
                ;
        public Task<IEnumerable<string>> List(Type childType, IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null) => throw new NotImplementedException();
        public Task<IEnumerable<string>> List(IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null) => throw new NotImplementedException();
    }
}

