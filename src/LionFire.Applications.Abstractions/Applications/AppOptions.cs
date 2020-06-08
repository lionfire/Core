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
        public PerformanceMode PerformanceMode { get; set; } = PerformanceMode.HighPerformance;

        public UpdatePolicy UpdatePolicy { get; set; }

        #region StartingServices

        public IEnumerable<Type> StartingServices => startingServices;
        private List<Type> startingServices { get; } = new List<Type>();

        public AppOptions AddStartingService<T>()
                  where T : IHostedService
        {
            startingServices.Add(typeof(T));
            return this;
        }

        #endregion

        #region HostedServices

        public IEnumerable<Type> HostedServices => hostedServices;
        private List<Type> hostedServices { get; } = new List<Type>();

        public AppOptions AddHostedService<T>()
            where T : IHostedService
        {
            hostedServices.Add(typeof(T));
            return this;
        }

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
