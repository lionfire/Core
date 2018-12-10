using LionFire.ObjectBus;
using LionFire.ObjectBus.RedisPub;
using LionFire.Referencing;

namespace LionFire.Applications.Hosting
{
    public static class AppHostRedisPubObjectBusExtensions
    {
        public static IAppHost AddRedisPubObjectBus(this IAppHost app)
        {
            app.TryAddEnumerableSingleton<IOBus, RedisPubOBus>();

            return app;
        }
    }
}
