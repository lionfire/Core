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
using LionFire.Testing;

namespace LionFire.Persisters.SharpZipLib.Tests;

[TestClass]
public static class TestHostBuilder
{

    public static string DataDir
    {
        get
        {
            if (dataDir == null)
            {
                var loc = typeof(TestHostBuilder).Assembly.Location;
                var dir = Path.GetDirectoryName(loc);
                dataDir = Path.GetFullPath("../../../../../test/data", dir);
                dataDir = Path.Combine(dataDir, typeof(TestHostBuilder).Assembly.GetName().Name);
            }
            return dataDir;
        }
    }
    private static string dataDir;

    [AssemblyInitialize]
    public static void Init(TestContext testContext)
    {
        TestRunner.HostApplicationFactory = () =>
        {
            var hab = new HostApplicationBuilder();

            hab.LionFire(lf => lf
                .Vos()
            )
            .Services
                .AddFilesystemPersister()
                .VosMount("/C", "c:".ToFileReference()) // TEMP
                .AddSharpZipLibPersistence()
#warning NEXT: Use the CanDeserializeType on the Strategy selector, trace through the Serializer selector code
            ;
            return hab;
        };
    }
}
