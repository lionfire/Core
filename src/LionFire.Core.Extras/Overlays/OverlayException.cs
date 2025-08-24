using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Overlays;

[Serializable]
public class OverlayException : Exception
{
    public OverlayException() { }
    public OverlayException(string message) : base(message) { }
    public OverlayException(string message, Exception inner) : base(message, inner) { }
}

