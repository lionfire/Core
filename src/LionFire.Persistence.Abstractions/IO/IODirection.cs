using System;

namespace LionFire.IO
{
    [Flags]
    public enum IODirection
    {
        Unspecified = 0,
        Write = 1 << 0,
        Read = 1 << 1,
        ReadWrite = Read | Write,
    }
   
}
