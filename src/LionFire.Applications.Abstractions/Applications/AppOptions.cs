//using AppUpdate;
//using AppUpdate.Common;

using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Applications
{

    public class AppOptions
    {
        //public PerformanceMode PerformanceMode { get; set; } = PerformanceMode.HighPerformance;

        #region StartingServices

#if OLD // Use DependencyMachine
        public IEnumerable<Type> StartingServices => startingServices;
        private List<Type> startingServices { get; } = new List<Type>();

        public AppOptions AddStartingService<T>()
                  where T : IHostedService
        {
            startingServices.Add(typeof(T));
            return this;
        }
#endif

#endregion

#region HostedServices
        // Use DependencyMachine.AddHostedServiceDependency instead

#if OLD
        /// <summary>
        /// A list of IHostedServices that will be started (and stopped) in order
        /// </summary>
        public List<Type> HostedServices => hostedServices;

        private List<Type> hostedServices { get; } = new List<Type>();

        //public AppOptions AddHostedService<T>()
        //    where T : IHostedService
        //{
        //    hostedServices.Add(typeof(T));
        //    return this;
        //}
        public AppOptions SetHostedServices(params Type[] types)
        {
            hostedServices.Clear();
            foreach (var s in types)
            {
                if (!typeof(IHostedService).IsAssignableFrom(s)) throw new ArgumentException($"{nameof(types)} must be assignable to {typeof(IHostedService)}");
                hostedServices.Add(s);
            }
            return this;
        }
        public AppOptions SetHostedServicesIfEmpty(params Type[] types)
        {
            if(hostedServices.Count == 0)
            {
                SetHostedServices(types);
            }
            return this;
        }
#endif

#endregion

        //#region StartAction

        //public Func<IServiceProvider, CancellationToken, Task> StartAction
        //{
        //    get { return startAction; }
        //    set
        //    {
        //        if (startAction == value) return;
        //        if (value != null && startAction != default) throw new NotSupportedException("StartAction can only be set once or back to null.");
        //        startAction = value;
        //    }
        //}
        //private Func<IServiceProvider, CancellationToken, Task> startAction;

        //#endregion

        //#region StopAction

        //public Func<IServiceProvider, CancellationToken, Task> StopAction
        //{
        //    get { return stopAction; }
        //    set
        //    {
        //        if (stopAction == value) return;
        //        if (value != null && stopAction != default) throw new NotSupportedException("StopAction can only be set once or back to null.");
        //        stopAction = value;
        //    }
        //}
        //private Func<IServiceProvider, CancellationToken, Task> stopAction;

        //#endregion
    }
}
