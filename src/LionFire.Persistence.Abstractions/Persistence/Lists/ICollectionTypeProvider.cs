#nullable enable
using LionFire.Referencing;
using System;

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
}
