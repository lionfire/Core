using LionFire.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#if true
new HostApplicationBuilder(args)
#else
Host.CreateDefaultBuilder(args)
#endif
    .LionFire()
    .Run(sp => ActivatorUtilities.CreateInstance<AppInfoFromConfiguration>(sp).DumpToLog());