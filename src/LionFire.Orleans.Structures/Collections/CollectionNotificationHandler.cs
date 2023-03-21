namespace LionFire.Orleans_.Collections;

public class CollectionNotificationHandler : ICollectionNotificationHandler
{
    public Action<string>? Added;
    public Action<string>? Removed;
    public Action<string>? Changed;
    public Action<CollectionChangedEvent>? Notified;

    public Task OnCollectionChanged(CollectionChangedEvent collectionChangedEvent)
    {
        Notified?.Invoke(collectionChangedEvent);

        switch (collectionChangedEvent.Type)
        {
            case CollectionChangeType.Added:
                foreach (var id in collectionChangedEvent.Ids) Added?.Invoke(id);
                break;
            case CollectionChangeType.Removed:
                foreach (var id in collectionChangedEvent.Ids) Removed?.Invoke(id);
                break;
            case CollectionChangeType.Changed:
                foreach (var id in collectionChangedEvent.Ids) Changed?.Invoke(id);
                break;
            default:
                break;
        }
        return Task.CompletedTask;
    }
}
