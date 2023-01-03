using LionFire.Hosting;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Persisters.Expanders;
using LionFire.Services;
using LionFire.Vos;
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
                var dir = Path.GetDirectoryName(loc);
                dataDir = Path.GetFullPath("../../../../../test/data", dir);
                dataDir = Path.Combine(dataDir, typeof(TestHostBuilder).Assembly.GetName().Name!);
            }
            return dataDir;
        }
    }
    private static string? dataDir;

    static string TestZipPath => Path.Combine(TestHostBuilder.DataDir, "TestSourceFile.zip");
    static string TestZipUrlPath => TestZipPath.Replace(":", "");

    public static readonly string ExpandMountReferenceString = $"expand:vos:///{TestZipUrlPath}:";

    public static IHostBuilder ExpandMount
        => H
                .ConfigureServices(s => s 
                    .VosMount("/test/ExpandMount", ExpandMountReferenceString.ToExpansionReference())
            );

    public static IHostBuilder H
    {
        get
        {
            return Host.CreateDefaultBuilder()
                .LionFire(lf => lf
                    .Vos()
                )
                .ConfigureServices(s => s
                    .Expansion()
                    .AddFilesystem()
                    .AddNewtonsoftJson()
                    .VosMount("/C", "c:".ToFileReference()) // TEMP - TODO: change to match location of test data dir

                    .AddSharpZipLib()
                    
                )
                ;
        }
    }
}
