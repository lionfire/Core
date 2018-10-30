using LionFire.ObjectBus;
using LionFire.ObjectBus.Filesystem;
using LionFire.Referencing;

namespace LionFire.Applications.Hosting
{
    public static class AppHostFsObjectBusExtensions
    {
        public static IAppHost AddFilesystemObjectBus(this IAppHost app)
        {
            app.TryAddEnumerableSingleton<IOBus, FsOBus>();
            //var obp = new FsOBus();
            //app.AddSingleton(obp);
            //app.AddSingleton<IHandleProvider, FilesystemOBus>();
            //app.AddSingleton<IReferenceProvider, FilesystemOBus>();
            //app.AddSingleton<IOBaseProvider, FilesystemOBus>();

            return app;
        }
    }
}
