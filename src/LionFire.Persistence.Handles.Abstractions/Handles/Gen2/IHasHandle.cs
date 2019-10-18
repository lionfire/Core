using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Persistence.Handles
{
    public interface IHasHandle
    {
        IHandleEx Handle { get;  }
    }
}
