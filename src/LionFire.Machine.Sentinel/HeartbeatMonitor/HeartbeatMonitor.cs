using LionFire.Instantiating;
using LionFire.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Supervising.Heartbeat
{

    public class HeartbeatMonitor : ITemplate<SHeartbeatMonitor>
    //: IKeyed<string>
    {
        //public string Key { get; set; }
        public AlarmPriority Priority { get; set; }
        public int SecondsTimeout { get; set; }
    }

}
