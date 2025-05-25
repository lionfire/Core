using LionFire.Persistence;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Vos.Collections.ByType
{
    public class CollectionsByTypeListProvider : IListProvider<VobReference>
    {
        /// <summary>
        /// Scenarios covered by this:
        /// </summary>
        /// <param name="childType"></param>
        /// <param name="persister"></param>
        /// <param name="referencable"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<IEnumerable<IListing<T>>> List<T>(IPersister<VobReference> persister, IReferenceable<VobReference> referencable, ListFilter filter = null)
        {
            var vob = referencable.ReferenceableToVob();

            var manager = vob.TryGetNextVobNode<ICollectionsByTypeManager>(minDepth: 1, maxDepth: 1)?.Value;

            if (manager == null) { return null; }

            var childTypeName = manager.ToTypeName(typeof(T));

            if (childTypeName == null) { return null; }

            throw new NotImplementedException();

        }
    }
}
