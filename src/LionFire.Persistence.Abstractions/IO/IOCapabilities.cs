using System;

namespace LionFire.IO
{
    [Flags]
    public enum IOCapabilities
    {
        Unspecified = 0,
        ReadStream = 1 << 0,
        ReadString = 1 << 1,
        ReadBytes = 1 << 2,
        WriteStream = 1 << 3,
        WriteString = 1 << 4,
        WriteBytes = 1 << 5,
        All = ReadStream | ReadString | ReadBytes | WriteStream | WriteString | WriteBytes,
    }
}
