using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Applications
{
    public class ApplicationTelemetry : IHostedService
    {

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartTime = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        #region StartTime

        public DateTime StartTime
        {
            get => startTime;
            protected set
            {
                if (startTime == value) return;
                if (startTime != default(DateTime)) throw new AlreadySetException();
                startTime = value;
            }
        }
        private DateTime startTime;

        #endregion
    }
    
}
