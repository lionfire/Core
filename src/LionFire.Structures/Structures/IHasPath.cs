using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    public interface IHasPath // Currently used by Service RPC system
    {
        string Path { get; }
    }
}
