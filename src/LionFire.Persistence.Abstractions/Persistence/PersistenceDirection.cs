using System;

namespace LionFire.Referencing
{
    [Flags]
    public enum PersistenceDirection
    {
        Unspecified = 0,
        Read = 1 << 0,
        Write = 1 << 1,
        ReadAndWrite = Read | Write,
    }
}
