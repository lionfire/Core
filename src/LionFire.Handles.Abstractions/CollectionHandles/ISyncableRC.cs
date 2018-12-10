namespace LionFire.Referencing
{
    public interface ISyncableRC<out T, TListEntry> : RC<T, TListEntry>
        where  TListEntry : ICollectionEntry
    {
        bool IsReadSyncEnabled { get; set; }
    }
}
