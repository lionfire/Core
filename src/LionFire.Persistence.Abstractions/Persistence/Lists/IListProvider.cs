#nullable enable
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public interface IListProvider<TReference>
        where TReference : IReference
    {

#if NETSTANDARD_2_1
        /// <summary>
        /// List the names of all children of a specified type
        /// </summary>
        /// <param name="persister"></param>
        /// <param name="referencable"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> List<TChildValue>(IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null)
            => List(typeof(TChildValue), persister, referencable, filter);
#endif

        /// <summary>
        /// Lists default type for the reference, or all objects if there is no default.
        /// </summary>
        /// <param name="persister"></param>
        /// <param name="referencable"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<Listing<T>>> List<T>(IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null);
    }
}
