using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Services.Hosting
{
    
    
    public class ServiceHost
    {
        public void Start(ServiceConfig sc)
        {
            var state = new ServiceState()
            {
                Config = sc,
            };

            state.Service = ServiceActivator.Activate(sc);

            //IDisposable disposable = service as IDisposable;
        }
    }
    public class ServiceActivator
    {
        public static object Activate(ServiceConfig sc)
        {
            var a = Assembly.Load(new AssemblyName("LionFire.Services.TestService"));
            var type = a.GetType("LionFire.Services.TestService.MyTestService");

            string testParam = "123";
            var service = Activator.CreateInstance(type, testParam);
            return service;
        }
    }
}
