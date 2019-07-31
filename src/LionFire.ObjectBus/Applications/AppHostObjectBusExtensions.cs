using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Applications.Hosting
{
    public static class AppHostObjectBusExtensions
    {
        public static IAppHost AddObjectBus(this IAppHost app)
        {
            #region 190708 - don't like this approach anymore, at the moment.  Want to try going pure IHandleProvider, not OBus provider.

            //app.AddSingleton<IReferenceToOBaseService, ReferenceToOBaseService>();
            //app.AddSingleton<IOBusReferenceProviderService, OBusReferenceProviderService>();

            #endregion
            return app;
        }

        public static IServiceCollection AddObjectBus(this IServiceCollection app)
        {
            #region 190708 - don't like this approach anymore, at the moment.  Want to try going pure IHandleProvider, not OBus provider.

            //app.AddSingleton<IReferenceToOBaseService, ReferenceToOBaseService>();
            //app.AddSingleton<IOBusReferenceProviderService, OBusReferenceProviderService>();
            
            #endregion

            return app;
        }
    }
}
