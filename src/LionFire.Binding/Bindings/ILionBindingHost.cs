using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Bindings
{
    public interface ILionBindingHost
    {
        List<LionBindingNode> Bindings { get; }
    }
}
