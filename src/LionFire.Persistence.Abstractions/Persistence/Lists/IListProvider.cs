#nullable enable
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    /// <remarks>
    /// When trying to write types other than the collection, persister should either write it as out of band data, or throw a persistence not supported exception if the out of band data is not supported, an example of which would be when a directory is mapped to a RDBMS table and no other data storage is available via a secondary Vos overlay.
    /// </remarks>
    public interface ICollectionTypeProvider<TReference>
        where TReference : IReference
    {
        Type GetCollectionType(TReference reference);
    }

    public class ObjectCollectionTypeProvider<TReference> : ICollectionTypeProvider<TReference>
        where TReference : IReference

    {
        public Type GetCollectionType(TReference reference) => typeof(object);
    }

    public class SpecifiedCollectionTypeProvider<TReference> : ICollectionTypeProvider<TReference>
        where TReference : IReference
    {
        public Type GetCollectionType(TReference reference)
        {
            // TODO: Deserialize ..Collection.*
            // TODO: Deserialize ..Collection Type=""
            // TODO: Deserialize ..Collection <Type>
            //return typeof(object);
            throw new NotImplementedException();
        }
    }
   

    

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
        Task<IEnumerable<string>> List(Type childType, IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null);

        /// <summary>
        /// Lists default type for the reference, or all objects if there is no default.
        /// </summary>
        /// <param name="persister"></param>
        /// <param name="referencable"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> List(IPersister<TReference> persister, IReferencable<TReference> referencable, ListFilter? filter = null);
    }
}
