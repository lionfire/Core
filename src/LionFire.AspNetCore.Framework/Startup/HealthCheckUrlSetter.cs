//using HealthChecks.UI.Configuration;
//using LionFire.AspNetCore;
//using Microsoft.AspNetCore.Hosting.Server;
//using Microsoft.AspNetCore.Hosting.Server.Features;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Serilog;
//using System.Diagnostics;
//using System.Threading;

//public class HealthCheckUrlSetter : IHostedService
//{
//    public HealthCheckUrlSetter(IServer server, IOptions<Settings> settings, ILogger<HealthCheckUrlSetter> logger)
//    {
//        Server = server;
//        Settings = settings;
//        Logger = logger;


//    }

//    public IServer Server { get; }
//    public IOptions<Settings> Settings { get; }
//    public ILogger<HealthCheckUrlSetter> Logger { get; }

//    public Task StartAsync(CancellationToken cancellationToken)
//    {
//        _ = Task.Run(async () =>
//        {

//        var addressFeature = Server.Features.Get<IServerAddressesFeature>();

//        var sw = Stopwatch.StartNew();
//        while ((addressFeature == null || !addressFeature.Addresses.Any()) && sw.Elapsed < TimeSpan.FromSeconds(30))
//        {
//            await Task.Delay(100);
//            addressFeature = Server.Features.Get<IServerAddressesFeature>();
//        }

//        if (addressFeature == null || !addressFeature.Addresses.Any())
//        {
//            Logger?.LogWarning("HealthChecksUI endpoint could not be determined.");
//            return;
//        }

//        var kestrelAddress = addressFeature.Addresses.First();
//        var healthCheckUrl = $"{kestrelAddress}/health";

//        var setup = Settings.Value;

//        var first = addressFeature.Addresses.First();

//        Logger?.LogInformation("HealthChecksUI endpoint: {0}", first);
//        //setup.ConfigureApiEndpointHttpclient((services, client) =>
//        //{
//        //    var logger = services.GetService<ILogger<WebFrameworkConfig>>();

//        //    first = first.Replace("0.0.0.0", "127.0.0.1");
//        //    Logger?.LogInformation("HealthChecksUI effective endpoint: {0}", first);
//        //    client.BaseAddress = new Uri(first);

//        //});

//        if (first.EndsWith("/")) { first = first[0..^1]; }

//        setup.AddHealthCheckEndpoint("self-detail", first + "/health/detail");

//        //Settings.Value.ConfigureApiEndpointHttpclient()
//        //healthCheckUiOptions.Value.AddHealthCheckEndpoint("My API", healthCheckUrl);
//        //Console.WriteLine($"HealthChecks.UI configured to use: {healthCheckUrl}");
//        });
//        return Task.CompletedTask;
//    }

//    public Task StopAsync(CancellationToken cancellationToken)
//    {
//        return Task.CompletedTask;
//    }
//}