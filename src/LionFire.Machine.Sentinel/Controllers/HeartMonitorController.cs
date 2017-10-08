using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LionFire.Execution.Executables;
//using LionFire.Instantiating;

namespace LionFire.Machine.Sentinel.Controllers
{
    public enum Priority
    {
        Unspecified = 0,
        Wake = 1,
        Urgent = 2,
        Info = 3,
    }

    public class HeartbeatOptions
    {
        public Priority Priority { get; set; }
        public int SecondsTimeout { get; set; }
    }

    public class HeartMonitorService : ExecutableBase
        //, IInstanceFor<HeartbeatOptions>
    {
        public HeartbeatOptions Template { get; set; }
    }

    //public class VocInstantiations<TTemplate, TInstance>
    //    where TTemplate : ITemplate
    //{

    //    public Vob Root { get; set; }

    //}

    public class HeartMonitorsService : ExecutableBase
    {
        //public VocInstantiations<HeartbeatOptions>
        
    }


    [Route("api/[controller]")]
    public class DiagController : Controller
    {
        [HttpGet("host")]
        public string Host()
        {
            return this.Request.Host.Host;
        }
    }

    [Route("api/[controller]")]
    public class HeartMonitorController : Controller
    {

        Dictionary<string, HeartMonitorService> h = new Dictionary<string, HeartMonitorService>();



        [HttpPut("{id}")]
        public string Register(string id, [FromBody] int secondsTimeout, [FromBody] int priority)
        {
            if (h.ContainsKey(id)) throw new Exception("Already exists: " + id);

            var service = new HeartMonitorService
            {
                Template = new HeartbeatOptions
                {
                    Priority = (Priority) priority,
                    SecondsTimeout = secondsTimeout,
                }
            };

            h.Add(id, service);

            return $"id:{id},sec:{secondsTimeout},pri:{priority}";
        }


        //public bool ()

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("get/{name}")]
        public string Get(string name)
        {
            return "todo";
        }

        [HttpPost("{id}")]
        public void Heartbeat([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
