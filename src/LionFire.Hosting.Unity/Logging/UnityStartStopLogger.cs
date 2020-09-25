using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LionFire.Hosting.Unity
{
    public class UnityStartStopLogger : IHostedService
    {
        public UnityStartStopLogger(ILogger<UnityStartStopLogger> logger)
        {
            Logger = logger;
        }

        private ILogger<UnityStartStopLogger> Logger { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //Debug.Log("UnityStartStopLogger: StartAsync");
            Logger.LogInformation("[log] UnityStartStopLogger: StartAsync");
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
