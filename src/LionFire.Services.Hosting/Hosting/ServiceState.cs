using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Services.Hosting
{
    public class ServiceState
    {
        public ServiceState()
        {
        }

        public ServiceConfig Config { get; set; }

        public object Service { get; set; }
        

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public bool IsDisposable {
            get { return Service as IDisposable != null; }
        }
    }

}
