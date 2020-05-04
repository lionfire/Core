using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Applications;
using LionFire.Services;

namespace LionFire.Services
{
    //[ServiceContract]
    public interface IService
    {        
        void Start();
        void Stop();

        /// <summary>
        /// Forcibly terminate
        /// </summary>
        void ForceStop(bool tryCleanForceStop = false);

        void Pause();
        void Continue();

        bool CanPauseAndContinue
        {
            get;
        }

        ServiceState State
        {
            get;
        }

        event Action<IService> StateChanged;

        
    }

}
namespace LionFire.Applications
{
    public static class IServiceExtensions
    {
        // FUTURE
        //public static void SetService<IServiceType>(this ILionFireApp app)
        //    where IServiceType : IService, new()
        //{
        //    var service = new IServiceType();
        //    app.Set<IService>(service);
        //    app.Start
        //}
        //public static IServiceType GetService<IServiceType>(this ILionFireApp app)
        //{

        //}
    }
}
