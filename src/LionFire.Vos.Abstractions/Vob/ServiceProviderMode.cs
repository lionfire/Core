using System;

namespace LionFire.Vos
{
    [Flags]
    public enum ServiceProviderMode
    {
        None = 1 << 0,
        UseRootManager = 1 << 1,
    }
}
