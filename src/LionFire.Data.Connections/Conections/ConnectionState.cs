using System;

namespace LionFire.Data.Connections
{
    [Flags]
    public enum ConnectionState
    {
        Unspecified = 0,
        NotConnected = 1 << 0,
        Connected = 1 << 1,
        Ready = 1 << 2,
        Disposed = 1 << 3,
        Faulted = 1 << 4,
    }
}
