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
                dataDir = Path.Combine(dataDir, typeof(TestHostBuilder).Assembly.GetName().Name);
            }
            return dataDir;
        }
    }
    private static string dataDir;

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
                    .AddNewtonsoftJson()
                     .VosMount("/C", "c:".ToFileReference()) // TEMP
                    .ArchiveAdapter("/") // TEMP
                    .AddSharpZipLib()
                    .TryAddEnumerableSingleton<IArchivePlugin, ZipPlugin>()

                    // How to convert this to something else?  "vos://c/temp/TestDoc.zip:/"
                    // ExpansionReference
                    //  - source: vos://c/temp/TestDoc.zip
                    //  - relative path: /
                    // .ToExpansionReference()  expand:<source>:<target>

                    // ^^^ DONE ^^^

                    // ExpansionProvider
                    // - source requirements: 

                    // Example usage
                    // Retrieve<string>("vos://testzip/myfile.txt")

                    // mount: vos://testzip > vos://c/temp/testzip.zip:/ 
                    // vos://c/temp/testzip.zip:/ is expand:vos://c/temp/testzip.zip:/
                    // so mapped request is: expand:vos://c/temp/testzip.zip:/myfile.txt (string)   REVIEW - can I convert reference types like this? Or is Expand built into VobReference?
                    // expansion providers by extension: "zip" => SharpZipLibExpansionProvider : ArchiveExpansionProvider
                    // SharpZipLibExpansionProvider get string:
                    //   - Archive Source: vos://c/temp/testzip.zip > Vob, but don't need to handle it?
                    //     - resolve to native handle, for performance? PersisterReadHandle<IFileReference, byte[], FilePersister>("file://c:/temp/testzip.zip")
                    //   - ZipEntry: "/myfile.txt"

                    // - Retrieve<byte[]>("vos://c/temp/testzip.zip")

                    // ArchiveMountOptions
                    //  - KeepOpenMillisecondsTimeout:  0 close immediately, null indefinite
                    //  - PreloadListing
                    //  - Read sharing mode
                    //  - Writable

                    .VosMount("/testzip", "vos://c/temp/TestDoc.zip:/".ToVobReference())
                //.AddVosObjectExplorerRoot() // TODO
                //.VosMount("/testmount".ToVobReference(), @"C:\temp".ToFileReference())
                )
                ;
        }
    }
}
