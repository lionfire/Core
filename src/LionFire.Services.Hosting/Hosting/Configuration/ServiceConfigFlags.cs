using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Services.Hosting
{
    public enum ServiceConfigFlags
    {
        None = 0,
        IsolatedProcess = 1 << 0,
        DisallowRestart = 1 << 1,

    }
}
