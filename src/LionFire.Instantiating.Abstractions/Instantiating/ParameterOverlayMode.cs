using System;

namespace LionFire.Instantiating
{
    [Flags]
    public enum ParameterOverlayMode
    {
        Unspecified = 0,
        None = 0,
        Children = 0x01,
        Properties = 0x02,
    }
}

