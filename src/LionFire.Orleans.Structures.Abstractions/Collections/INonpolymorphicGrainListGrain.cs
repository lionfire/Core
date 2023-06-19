#if false // OLD, unneeded optimization?

using LionFire.Data.Async.Collections;
using LionFire.Orleans_.Collections.ListGrain;

namespace LionFire.Orleans_.Collections;

/// <summary>
/// List of potentially various types of Grains that are all assignable to TGrainItem
/// </summary>
/// <typeparam name="TGrainItem"></typeparam>
public interface INonpolymorphicGrainListGrain<TGrainItem> 
    : ICreatingAsyncDictionary<string, GrainListGrainItem<TGrainItem>>
    where TGrainItem : IGrainWithStringKey
{
    Task<IEnumerable<TGrainItem>> GrainItems();
}
#endif