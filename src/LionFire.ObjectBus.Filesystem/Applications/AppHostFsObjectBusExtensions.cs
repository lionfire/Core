using LionFire.ObjectBus;
using LionFire.ObjectBus.Filesystem;
using LionFire.Referencing;

namespace LionFire.Applications.Hosting
{
    public static class AppHostFsObjectBusExtensions
    {
        public static IAppHost AddFilesystemObjectBus(this IAppHost app)
        {
            var obus = new FsOBus();
            var obp = new FsOBaseProvider();

            app.AddSingleton<IOBaseProvider>(obp);
            app.AddSingleton(obp);
            app.AddSingleton<IHandleProvider, FsOBaseHandleProvider>();
        }
        //{
            

        //    //return app.AddInit(_ =>
        //    //{
        //    //    // TODO TOUNGLOBAL: Remove global
        //    //    OBaseSchemeBroker.Instance.Register(FsOBaseProvider.Instance); // HARDCODE HARDCONF
        //    //    return true;
        //    //});
        //}
    }
}
