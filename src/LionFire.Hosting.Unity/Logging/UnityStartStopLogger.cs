using LionFire.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LionFire.Hosting.Unity
{
    public class UnityStartStopLogger : IHostedService, IAppStartLogger
    {
        public UnityStartStopLogger(ILogger<UnityStartStopLogger> logger)
        {
            Logger = logger;
        }

        public bool? LogSucceeded { get; private set; } = null;

        public object LogError { get; private set; }

        private ILogger<UnityStartStopLogger> Logger { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //Debug.Log("UnityStartStopLogger: StartAsync");
            try
            {
                Logger.LogInformation("[log] UnityStartStopLogger: StartAsync");
                LogSucceeded = true;
            }
            catch(Exception ex)
            {
                LogError = ex;
            }
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            //Debug.Log("UnityStartStopLogger: StopAsync");
            Logger.LogInformation("[log] UnityStartStopLogger: StopAsync");
            return Task.CompletedTask;
        }
    }

}
