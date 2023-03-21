using LionFire.Orleans_.Collections.PolymorphicGrainListGrain;

namespace LionFire.Orleans_.Collections;

/// <summary>
/// List of potentially various types of Grains that are all assignable to TGrainItem
/// </summary>
/// <typeparam name="TGrainItem"></typeparam>
public interface IPolymorphicGrainListGrain<TGrainItem>
    : IGrain
    , IListAsync<TGrainItem>
    , ICreatesAsync<TGrainItem>
    //ICreatingAsyncDictionary<string, PolymorphicGrainListGrainItem<TGrainItem>> 
    //, IEnumerable<TGrainItem>
    where TGrainItem : IGrain
{
    //IListGrain<string, PolymorphicGrainListGrainItem<TGrainItem>> ListGrain { get; }

    //Task<IEnumerable<TGrainItem>> GrainItems();

    public Task Subscribe(ICollectionNotificationHandler observer);

    public Task Unsubscribe(ICollectionNotificationHandler observer);

}
