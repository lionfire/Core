using LionFire.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Reflection;


Host.CreateApplicationBuilder(args)
//Host.CreateDefaultBuilder(args)
    .LionFire(lf =>
    {
        lf.ConfigureServices(services =>
        {
            services.AddHostedService<LoggingTest>();
        });
    })
    .Build()
    .Run();

class LoggingTest : IHostedService
{
    public LoggingTest(ILogger<LoggingTest> logger)
    {
        Logger = logger;
    }

    public ILogger<LoggingTest> Logger { get; }

    static int context = 0;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("LoggingTest.StartAsync");

        Task.Run(async () =>
        {
            var t = new PeriodicTimer(TimeSpan.FromSeconds(5));
            while (isRunning)
            {
                var sensorInput = new { Latitude = 25, Longitude = 134 };
                using (LogContext.PushProperty("ContextId", context++))
                {

                    //Logger.LogInformation("Processing {@SensorInput}", sensorInput);

                    Logger.LogInformation("LoggingTest.Log {flags}", BindingFlags.Public);
                    await t.WaitForNextTickAsync();
                }
            }
        });
        return Task.CompletedTask;
    }

    bool isRunning = true;
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("LoggingTest.StopAsync");
        isRunning = false;
        return Task.CompletedTask;
    }
}
