using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{

    public interface IHasReadHandle
    {
        RH<object> ReadHandle { get; }
    }
}
