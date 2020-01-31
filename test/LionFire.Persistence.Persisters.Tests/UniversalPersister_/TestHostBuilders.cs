//using LionFire.ObjectBus.Filesystem.Persisters;
using Microsoft.Extensions.Hosting;
using LionFire.Applications.Hosting;
using LionFire.Persistence;
using LionFire.Services;

namespace LionFire
{
    public partial class TestHostBuilders
    {
        public static IHostBuilder CreateFileNewtonsoftHost()
                  => PersistersHost.Create()
                                     .ConfigureServices(s =>
                                     {
                                         s
                                         .AddNewtonsoftJson()
                                         .AddFilesystem()
                                         ;
                                     });
        public static IHostBuilder CreateFileHost()
          => PersistersHost.Create()
                             .ConfigureServices(s =>
                             {
                                 s
                                 .AddFilesystem()
                                 ;
                             });
    }
}
