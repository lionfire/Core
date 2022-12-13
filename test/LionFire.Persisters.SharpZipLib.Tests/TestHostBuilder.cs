using LionFire.Hosting;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Persisters.Expanders;
using LionFire.Serialization;
using LionFire.Services;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using LionFire.Persisters.SharpZipLib.Tests.Deserialize;

namespace LionFire.Persisters.SharpZipLib.Tests;

public static class TestHostBuilder
{
    public static IHostBuilder H
    {
        get
        {
            return Host.CreateDefaultBuilder()
                .LionFire(lf => lf
                    .Vos()
                )
                .ConfigureServices((c, s) => s
                    .AddFilesystem()
                    .VosMount("/c", "c:".ToFileReference()) // TEMP
                    .AddSharpZipLibPersistence()
#warning NEXT: Use the CanDeserializeType on the Strategy selector, trace through the Serializer selector code
                )
                ;
        }
    }
}
