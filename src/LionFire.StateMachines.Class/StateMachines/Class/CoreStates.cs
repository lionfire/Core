using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.StateMachines.Class.States
{
    [Flags]
    public enum CoreStates
    {
        Uninitialized = 0,
        Disposed = 1 << 0,
        Faulted = 1 << 1,
        test2 = X.test1,
    }

    public enum X
    {
        test1 = 123,
    }
}
