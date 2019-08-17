namespace LionFire.Persistence.Handles
{
    public interface ISyncDifference
    {
        SyncDifferenceStatus Status { get; }

        ISyncConflict Parent { get; }
        object Theirs { get; }
        object Mine { get; }
    }



}
