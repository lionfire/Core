using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Applications
{
    public enum AppState // REVIEW - Use ServiceState instead?
    {
        Uninitialized = 0,
        Starting,
        Started,
        Pausing,
        Paused,
        Continuing,
        Stopping,
        Stopped,
        Disposed,
    }
}
