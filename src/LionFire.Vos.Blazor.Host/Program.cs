using LionFire.Hosting;
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
            .ConfigureServices(s=>s
                .ArchivePlugin("/"))
            //.AddBlazorise(options =>
            //{
            //    options.ChangeTextOnKeyPress = true;
            //})

            .Build()
            .Run();
    }
}
