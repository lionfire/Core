using LionFire.Hosting;
using LionFire.Persistence.Filesystem;
using LionFire.Persisters.Expanders;
using LionFire.Services;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

public static class TestHostBuilder
{
    public static IHostBuilder H 
        => Host.CreateDefaultBuilder()
                .LionFire(lf => lf
                    .Vos()
                )
                .ConfigureServices(s => s
                    .Expansion()
                    .AddFilesystem()
                );
}
