using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus // RENAME to LionFire.Handles? or LionFire.Referencing?
{
    public interface IHasReadHandle
    {
        IReadHandle ReadHandle { get; }
    }
}
