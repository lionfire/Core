using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Applications.Hosting
{
    public static class AppHostObjectBusExtensions
    {
        public static IAppHost AddObjectBus(this IAppHost app)
        {
            app.AddSingleton<IReferenceToOBaseService, ReferenceToOBaseService>();
            app.AddSingleton<IOBusReferenceProviderService, OBusReferenceProviderService>();

            return app;
        }
    }
}
