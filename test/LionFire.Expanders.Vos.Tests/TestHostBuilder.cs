using LionFire.Hosting;
using LionFire.Persistence.Filesystem;
using LionFire.Persisters.Expanders;
using LionFire.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Templates.Themes;
using Serilog.Templates;
using Serilog.Sinks.Loki;
using Serilog.Sinks.Loki.Labels;
using Serilog.Sinks.File.GZip;
using Serilog.Formatting;
using LionFire.Persistence.Handles;
using LionFire.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TestHostBuilder
{
    public static string DataDir
    {
        get
        {
            if (dataDir == null)
            {
                var loc = typeof(TestHostBuilder).Assembly.Location;
                var dir = Path.GetDirectoryName(loc) ?? throw new Exception("Couldn't get Assembly Location");
                dataDir = Path.GetFullPath("../../../../../test/data", dir);
                //dataDir = Path.Combine(dataDir, typeof(TestHostBuilder).Assembly.GetName().Name!);
            }
            return dataDir;
        }
    }
    private static string? dataDir;

    static string TestZipPath => Path.Combine(DataDir, "zip/TestSourceFile.zip");
    static string TestZipUrlPath => TestZipPath.Replace(":", "");

    public static readonly string ZipBaseVobReferenceString = $"vos:///{TestZipUrlPath}:";

    [AssemblyInitialize]
    public static void Init(TestContext testContext)
    {
        TestRunner.HostApplicationFactory = () =>
            {
                return new HostApplicationBuilder(new HostApplicationBuilderSettings() { })
                    .LionFire(lf => lf
                        .Vos()
                        .ConfigureServices(s => s
                            .Expansion()
                            .AddFilesystem()

                            .TryAddEnumerableSingleton<IExpanderPlugin, ZipPlugin>()

                            .AddExpanderMounter("/testdata/zip")
                            .VosMount("/testdata", DataDir.ToFileReference())

                            .AddSharpZipLib()
                            .AddNewtonsoftJson()
                        )
                    );
            };
    }

    //[TestMethod]
    //public void X()
    //{
    //    Assert.IsTrue(true);
    //}

}
