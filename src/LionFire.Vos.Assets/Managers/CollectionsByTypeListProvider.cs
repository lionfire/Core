using LionFire.Persistence;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Vos.Collections.ByType
{
    public class CollectionsByTypeListProvider : IListProvider<VosReference>
    {
        /// <summary>
        /// Scenarios covered by this:
        /// </summary>
        /// <param name="childType"></param>
        /// <param name="persister"></param>
        /// <param name="referencable"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<IEnumerable<Listing>> List(Type childType, IPersister<VosReference> persister, IReferencable<VosReference> referencable, ListFilter filter = null)
        {
            var vob = referencable.ReferencableToVob();

            var manager = vob.TryGetNextVobNode<ICollectionsByTypeManager>(minDepth: 1, maxDepth: 1)?.Value;

            if(manager == null) { return null; }

            var childTypeName = manager.ToTypeName(childType);

            if(childTypeName == null) { return null; }

            throw new NotImplementedException();

        }

        public Task<IEnumerable<Listing>> List(IPersister<VosReference> persister, IReferencable<VosReference> referencable, ListFilter filter = null) => throw new NotImplementedException();
    }

}
