using System;

namespace LionFire.ObjectBus.Notifications
{
    [Flags]
    public enum OBaseNotificationType
    {
        Unspecified = 0,
        Changed = 1 << 0,
        Deleted = 1 << 1,
        Created = 1 << 2,
        Renamed = 1 << 3,
        Moved = 1 << 4,
        Conflicted = 1 << 5,
        ConflictResolved = 1 << 6,
        Merged = 1 << 7,
        Synced = 1 << 8,
        BecameOutOfSync = 1 << 9,
    }
}
