#nullable enable
using LionFire.Referencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters;

public interface IListPersister<in TReference>
    where TReference : IReference
{
    /// <summary>
    /// List the names of all children
    /// </summary>
    /// <param name="referencable"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<IGetResult<IEnumerable<IListing<T>>>> List<T>(IReferencable<TReference> referencable, ListFilter? filter = null);
}
