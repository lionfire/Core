namespace LionFire.Persistence.Handles
{
    public interface IWatchableRC<T, TListEntry> : RC<T, TListEntry>
        where TListEntry : ICollectionEntry
    {
        bool IsReadSyncEnabled { get; set; }
    }
}
