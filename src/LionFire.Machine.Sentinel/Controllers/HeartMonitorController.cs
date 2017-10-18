using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LionFire.Execution.Executables;
using LionFire.Structures;
using LionFire.Serialization;
using LionFire.Persistence;
using LionFire.Supervising.Heartbeat;
using System.IO;
using LionFire.ExtensionMethods;
//using LionFire.Instantiating;

namespace LionFire.Machine.Sentinel.Controllers
{

    //public class VocInstantiations<TTemplate, TInstance>
    //    where TTemplate : ITemplate
    //{
    //    public Vob Root { get; set; }
    //}

    public class AssetPersistenceController<TAsset> : Controller
        where TAsset:class
    {
        PersistencePipeline<TAsset> pipeline;
        FsObjectCollection<TAsset> fsObjects = new FsObjectCollection<TAsset>();

        public AssetPersistenceController()
        {
            pipeline = Defaults.Get<PersistencePipeline<TAsset>>();
            fsObjects.RootPath = Path.Combine(LionFireEnvironment.AppProgramDataDir, "HeartMonitors"); // TODO: Use asset ifranstructure to set this more automatically
            if(fsObjects.Handles.Count == 0)
            {
                Put("testonempty", Activator.CreateInstance<TAsset>());
            }
        }

        // GET api/type
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return fsObjects.Handles.Keys.ToArray();
        }

        // GET api/type/5
        [HttpGet("get/{key}")]
        public TAsset Get(string key)
        {
            var existing = fsObjects.Handles[key];
            if (existing != null && existing.Object != null)
            {
                return existing.Object;
            }
            return null;
        }
        
        [HttpPost("{key}")]
        public void Post(string key, [FromBody]TAsset value)
        {
            var existing = fsObjects.Handles[key];
            if (existing != null && existing.Object != null)
            {
                existing.Object.AssignPropertiesFrom(value); // TODO - only assign non-default properties
                existing.Save();
            }
            else
            {
                fsObjects.Add(value, key);
            }
        }

        // PUT api/type/5
        [HttpPut("{key}")]
        public void Put(string key, [FromBody]TAsset value)
        {
            var existing = fsObjects.Handles[key];
            if (existing != null && existing.Object != null)
            {
                throw new AlreadyException("Already exists");
            }
            else
            {
                fsObjects.Add(value, key);
            }
        }

        // DELETE api/type/5
        [HttpDelete("{key}")]
        public void Delete(string key)
        {
            if(fsObjects.Handles.ContainsKey(key))
            {
                fsObjects.Remove(key);
                //fsObjects.Handles[key].Object = null;
            }
        }
    }

    public class PersistencePipeline<TObject>
    {

        public event Action<PersistenceContext> Validating;
        public event Action<PersistenceContext> ValidationFailed;

        public event Action<PersistenceContext> Created;
        public event Action<PersistenceContext> Updated;
        public event Action<PersistenceContext> Loaded;
        public event Action<PersistenceContext> Saved;

    }

    [Route("api/[controller]")]
    public class HeartMonitorController : AssetPersistenceController<HeartbeatMonitor>
    {

        //Dictionary<string, HeartbeatMonitor> h = new Dictionary<string, HeartbeatMonitor>();



        //[HttpPut("{id}")]
        //public string Register(string id, [FromBody] int secondsTimeout, [FromBody] int priority)
        //{
        //    if (h.ContainsKey(id)) throw new Exception("Already exists: " + id);

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


        //public bool ()

     
       

  
    }
}
