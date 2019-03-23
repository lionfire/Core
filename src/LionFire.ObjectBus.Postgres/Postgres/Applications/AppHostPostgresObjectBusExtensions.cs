using LionFire.ObjectBus;
using LionFire.ObjectBus.Postgres;
using LionFire.Referencing;

namespace LionFire.Applications.Hosting
{
    public static class AppHostPostgresObjectBusExtensions
    {
        public static IAppHost AddPostgresObjectBus(this IAppHost app)
        {
            app.TryAddEnumerableSingleton<IOBus, PostgresOBus>();

            return app;
        }
    }
}
