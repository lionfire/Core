using System;

namespace LionFire
{
    [Flags]
    public enum PersistenceEventSourceType
    {
        Unspecified = 0,
        Self = 1 << 0,
        Source = 1 << 1,
    }
}
