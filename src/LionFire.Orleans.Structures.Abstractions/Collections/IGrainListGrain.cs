using LionFire.Orleans_.Collections.ListGrain;

namespace LionFire.Orleans_.Collections;

/// <summary>
/// List of potentially various types of Grains that are all assignable to TGrainItem
/// </summary>
/// <typeparam name="TGrainItem"></typeparam>
public interface IGrainListGrain<TGrainItem> 
    : ICreatingAsyncDictionary<string, GrainListGrainItem<TGrainItem>>
    where TGrainItem : IGrainWithStringKey
{
    Task<IEnumerable<TGrainItem>> GrainItems();
}