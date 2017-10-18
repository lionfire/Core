using LionFire.Execution.Executables;
using LionFire.Instantiating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Supervising.Heartbeat
{
    public class SHeartbeatMonitor : ExecutableBase, ITemplateInstance<HeartbeatMonitor>
        //IInstanceFor<HeartbeatOptions>
    {
        public HeartbeatMonitor Template { get; set; }
    }
}
