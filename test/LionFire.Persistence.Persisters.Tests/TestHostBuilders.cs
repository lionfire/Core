//using LionFire.ObjectBus.Filesystem.Persisters;
using Microsoft.Extensions.Hosting;
using LionFire.Applications.Hosting;
using LionFire.Persistence;
using LionFire.Services;
using LionFire.Hosting;

namespace LionFire
{
    public partial class TestHostBuilders
    {
        public static IHostBuilder CreateFileNewtonsoftHost(IPersisterTestInitializer initializer = null)
                  => PersistersHost.Create()
                                     .ConfigureServices(s =>
                                     {
                                         s
                                         .AddNewtonsoftJson()
                                         .AddFilesystemPersister()
                                         ;
                                         if (initializer != null) initializer.AddServicesForTest(s);
                                     });

        public static IHostBuilder CreateFileHost()
          => PersistersHost.Create()
                             .ConfigureServices(s =>
                             {
                                 s
                                 .AddFilesystemPersister()
                                 ;
                             });
    }
}
