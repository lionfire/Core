using LionFire.Activities;
using LionFire.Hosting;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Persisters.Expanders;
using LionFire.Services;
using LionFire.UI;
using LionFire.Vos;
using LionFire.Vos.Blazor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System;

//Host.CreateDefaultBuilder(args) // Doesn't work
new HostApplicationBuilder(args)
    .LionFire(lf => lf
        .Vos()
        .WebHost<VosBlazorHostStartup>()
    )
    .ConfigureServices(s => s
        .Expansion()
        //.AddExpanderMounter("/fs/c/temp") // TEMP
        //.AddExpanderMounter("/fs") // TEMP
        .AddExpanderMounter("/") // TEMP
        .AddSharpZipLib()
        .TryAddEnumerableSingleton<IExpanderPlugin, ZipPlugin>() // REVIEW
        .AddSingleton<IActivitiesTracker, ActivitiesTracker>() // REVIEW


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

        //.VosMount("/testzip", "vos://c/temp/TestDoc.zip:/".ToVobReference())
        .VosMount("/fs/c", "C:/".ToFileReference())
    //.AddVosObjectExplorerRoot() // TODO
    //.VosMount("/testmount".ToVobReference(), @"C:\temp".ToFileReference())

    .AddOpenTelemetry()
        .WithMetrics(b => b
            .AddProcessInstrumentation()
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            //.AddHttpClientInstrumentation()
            .AddVosInstrumentation()
        
        .AddOtlpExporter(option =>
        {
            option.Endpoint = new Uri("http://localhost:4317");
            option.ExportProcessorType = ExportProcessorType.Batch;
            option.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        })
        )
        //.StartWithHost()
    )


    //.AddBlazorise(options =>
    //{
    //    options.ChangeTextOnKeyPress = true;
    //})

    .Build()
    .Run();

