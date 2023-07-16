using System;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Structures;
using LionFire.Execution.Roslyn.Scripting;
using LionFire.Execution.Hosting;

// TODO: Use a HostBuilder .LionFire()
ManualSingleton<IServiceCollection>.Instance = new ServiceCollection();

ManualSingleton<IServiceCollection>.Instance
    .AddRoslynScripting()
    ;

new ExecutionHostConsoleApp().Run(args);
