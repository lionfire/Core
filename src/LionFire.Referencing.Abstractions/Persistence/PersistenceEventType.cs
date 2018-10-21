using System;

namespace LionFire.Persistence
{
    [Flags]
    public enum PersistenceEventType
    {
        Unspecified = 0,
        Changed = 1 << 0,
        Created = 1 << 1,
        Deleted = 1 << 2,
        Moved = 1 << 3,
    }
}
