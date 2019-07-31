//using LionFire.ObjectBus;
//using LionFire.ObjectBus.Filesystem;
//using LionFire.Referencing;
//using Microsoft.Extensions.DependencyInjection;
//using LionFire.DependencyInjection;

//namespace LionFire.Applications.Hosting
//{
//    public static class AppHostFsObjectBusExtensions
//    {
//        public static IAppHost AddFilesystemObjectBus(this IAppHost app)
//        {
//            app.ServiceCollection.AddFilesystemObjectBus();
//            return app;
//        }
//        public static IServiceCollection AddFilesystemObjectBus(this IServiceCollection services)
//        {
//            services.AddObjectBus<FsOBus>();
//            return services;
//        }

//    }
//}
