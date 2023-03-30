using Orleans.Runtime;

namespace LionFire.Orleans_.Collections;

public interface ICollectionNotificationHandler //: IAddressable
{
    Task OnCollectionChanged(CollectionChangedEvent collectionChangedEvent);
}



[GenerateSerializer]
public class CollectionChangedEvent
{
    [Id(0)]
    public CollectionChangeType Type { get; set; }

    [Id(1)]
    public List<string> Ids { get; set; }
}



public enum CollectionChangeType
{
    Unspecified = 0,
    Added = 1,
    Removed = 2,
    Changed = 3,
}