
using LionFire.Hosting;
using LionFire.Hosting.CommandLine;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using LionFire.Applications;
using LionFire.Applications;
using Microsoft.Extensions.Logging;
using LionFire.ExtensionMethods.Dumping;

// TODO: Read https://github.com/dotnet/command-line-api/blob/main/src/System.CommandLine.Hosting.Tests/HostingHandlerTest.cs
// TODO: Read https://github.com/dotnet/command-line-api/blob/main/src/System.CommandLine.Hosting.Tests/HostingTests.cs

var p = new HostApplicationBuilderProgram()
   .DefaultArgs("run")
   //.RootCommand(b => b.LionFireAppInfo())
   //.AddOrleansCommands()
   .Command<RunCommandLineOptions>("run", (commandContext, builder) => builder
   //.If(commandContext.GetOptions<UniverseSiloCommandLineOptions>().Dev, b => b.Environment.EnvironmentName = "Development")
     .LionFire(5000, l => { })

     .Services.AddHostedService<RunBackgroundService>()
   );
// More typical:
//return await p.RunAsync(args); 

#region Sample setup:

async Task Run(params string[] args)
{
    var result = await p.RunAsync(args);
    Console.WriteLine($"{string.Join(',', args)} returned {result}");
}

#endregion

// Samples:
await Run("run");
await Run("run", "--dev");

public class RunCommandLineOptions
{
    public bool Dev { get; set; }
}

public class RunBackgroundService : BackgroundService
{
    public RunBackgroundService(AppInfo appInfo, ILogger<RunBackgroundService> logger
        //, RunCommandLineOptions runCommandLineOptions

        )
    {
        AppInfo = appInfo;
        Logger = logger;
        //RunCommandLineOptions = runCommandLineOptions;
    }

    public AppInfo AppInfo { get; }
    public ILogger<RunBackgroundService> Logger { get; }
    //public RunCommandLineOptions RunCommandLineOptions { get; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Logger.LogInformation($"RunCommandLineOptions: {RunCommandLineOptions.Dump()}");
        Logger.LogInformation($"AppInfo: {AppInfo.Dump()}");
        return Task.CompletedTask;
    }
}
