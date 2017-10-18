namespace LionFire.Synchronization
{
    public enum ConflictResolutionMode
    {
        Unspecified,
        DestinationWins,
        SourceWins,
        NewestWins,
        AutoMerge,
        ManualMerge,
    }

}
