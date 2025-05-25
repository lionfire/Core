#nullable enable
using DynamicData;
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
    /// <param name="referenceable"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<IGetResult<IEnumerable<IListing<T>>>> List<T>(IReferenceable<TReference> referenceable, ListFilter? filter = null);
}

// TODO
public interface IObservableListPersister<in TReference>
    where TReference : IReference
{
    // REVIEW: consider replacing filter with standard Rx api
    IObservableCache<IListing<T>, string> GetObservableList<T>(IReferenceable<TReference> referenceable, ListFilter? filter = null)
        where T : notnull;
}


