using System;

namespace LionFire.Persistence.Handles
{
    [Flags]
    public enum SyncDifferenceStatus
    {
        Unspecified = 0,
        TookTheirs = 1 << 0,
        TookMine = 1 << 1,
        ManuallyMerged = 1 << 2,
        AutoMerged = 1 << 3,
    }



}
