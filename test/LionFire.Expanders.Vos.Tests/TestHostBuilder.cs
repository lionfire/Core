using LionFire.Hosting;
using LionFire.Persistence.Filesystem;
using LionFire.Persisters.Expanders;
using LionFire.Services;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

public static class TestHostBuilder
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

    public static IHostBuilder H
    {
        get
        {
            return Host.CreateDefaultBuilder()
                .LionFire(lf => lf
                    .Vos()
                )
                .ConfigureServices((c, s) => s
                    .Expansion()
                    .AddFilesystem()

                    .TryAddEnumerableSingleton<IExpanderPlugin, ZipPlugin>()

                    .AddExpanderMounter("/testdata")
                    .VosMount("/testdata", DataDir.ToFileReference())

                    .AddSharpZipLib()
                    .AddNewtonsoftJson()

                )
                ;
        }
    }
}
