using LionFire.Dependencies;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Applications;

public class ApplicationTelemetry : IHostedService
{
    public static ApplicationTelemetry Current => DependencyContext.Current.GetService<ApplicationTelemetry>();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        StartTime = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        StopTime = DateTime.UtcNow;
        return Task.CompletedTask;
    }

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

    #region StopTime

    public DateTime StopTime
    {
        get => stopTime;
        protected set
        {
            if (stopTime == value) return;
            if (stopTime != default(DateTime)) throw new AlreadySetException();
            stopTime = value;
        }
    }
    private DateTime stopTime;

    #endregion
}
