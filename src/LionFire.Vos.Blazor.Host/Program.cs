using LionFire.Hosting;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Persisters.Expanders;
using LionFire.Services;
using LionFire.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LionFire.Vos.Blazor;

//NLog.LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

public class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .LionFire(lf => lf
                .Vos()
                .WebHost<VosBlazorHostStartup>()
            )
            .ConfigureServices((c,s) => s
                .ArchiveAdapter("/") // TEMP
                .AddSharpZipLib()
                .TryAddEnumerableSingleton<IArchivePlugin, ZipPlugin>() // REVIEW

                // How to convert this to something else?  "vos://c/temp/TestDoc.zip:/"
                // ExpansionReference
                //  - source: vos://c/temp/TestDoc.zip
                //  - relative path: /
                // .ToExpansionReference()  expand:<source>:<target>
                //  - if missing expand:, infer it
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

                .VosMount("/testzip","vos://c/temp/TestDoc.zip:/".ToVobReference())
            //.AddVosObjectExplorerRoot() // TODO
            //.VosMount("/testmount".ToVobReference(), @"C:\temp".ToFileReference())
            )
                

            //.AddBlazorise(options =>
            //{
            //    options.ChangeTextOnKeyPress = true;
            //})

            .Build()
            .Run();
    }
}
