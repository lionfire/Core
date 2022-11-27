using System;

namespace LionFire.IO;

[Flags]
public enum IODirection
{
    Unspecified = 0,
    Write = 1 << 0,
    Read = 1 << 1,
    ReadWrite = Read | Write,
}


public static class IODirectionExtensions
{
    public static bool IsReadable(this IODirection d) =>  d.HasFlag(IODirection.Read);
    public static bool IsWritable(this IODirection d) =>  d.HasFlag(IODirection.Write);
}