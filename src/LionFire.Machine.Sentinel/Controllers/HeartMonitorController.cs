using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LionFire.Execution.Executables;
using LionFire.Persistence;
using LionFire.Supervising.Heartbeat;
using LionFire.Persistence.Assets;
using LionFire.Assets;
//using LionFire.Instantiating;

namespace LionFire.Machine.Sentinel.Controllers
{
    [Route("api/[controller]")]
    public class HeartMonitorController : AssetPersistenceController<HeartbeatMonitor>
    {
        public HeartMonitorController()
        {
            if (assetObjects.Handles.Count == 0)
            {
                var testObj = Activator.CreateInstance<HeartbeatMonitor>();
                testObj.Priority = Notifications.AlarmPriority.Info;
                testObj.SecondsTimeout = 123;
                Put("testonempty", testObj).FireAndForget();
            }
        }
        //public string Register(string id, [FromBody] int secondsTimeout, [FromBody] int priority)
        //{
        //    var service = new HeartMonitorService
        //    {
        //        Template = new HeartbeatMonitor
        //        {
        //            Priority = (Priority)priority,
        //            SecondsTimeout = secondsTimeout,
        //        }
        //    };
        //    h.Add(id, service);
        //    return $"id:{id},sec:{secondsTimeout},pri:{priority}";
        //}

    }
}
