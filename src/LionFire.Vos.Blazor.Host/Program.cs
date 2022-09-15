using LionFire.Hosting;
using LionFire.Persistence.Filesystem;
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
