using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Services
{
    
    public interface IService
    {
        Task Start();
        Task Stop(StopMode mode, StopOptions options);
    }

    public interface IPausableService
    {
        Task Pause();
        Task Continue();
    }

    //public class ServiceBase : IService
    //{
    //    public ServiceBase()
    //    {
    //    }

        
    //}
}
