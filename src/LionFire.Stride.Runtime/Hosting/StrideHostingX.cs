using Microsoft.Extensions.DependencyInjection;
using Stride.Core.IO;
using Stride.Core.Storage;
using Stride.Core;
using Stride.Graphics;

namespace LionFire.Hosting;

public static class StrideHostingX
{
    public static IServiceCollection StrideRuntime(this IServiceCollection services)
    {
        ServiceRegistry GlobalStrideServices = new();
        services.AddSingleton<ServiceRegistry>(GlobalStrideServices);

        // Database file provider
        var objDb = ObjectDatabase.CreateDefaultDatabase();
        services.AddSingleton<ObjectDatabase>(objDb);
        var dbFileProvider = new DatabaseFileProvider(objDb);
        services.AddSingleton<DatabaseFileProvider>(dbFileProvider);
        var dbFileProviderService = new DatabaseFileProviderService(dbFileProvider);
        services.AddSingleton<DatabaseFileProviderService>(dbFileProviderService);

        GlobalStrideServices.AddService<IDatabaseFileProviderService>(dbFileProviderService);

        #region FUTURE: Global Content manager
        //var Content = new GlobalContentManager(GlobalStrideServices);
        ////var Content = new ContentManager(GlobalStrideServices); No, it's not shareable due to async thread scheduling
        //services.AddSingleton<ContentManager>(Content);
        //GlobalStrideServices.AddService<IContentManager>(Content);
        //GlobalStrideServices.AddService(Content);
        #endregion

        // Server only, maybe, but doesn't seem to be needed: MOVE
        // Enabling this breaks because Physics tries to add Debug Material code
        //GlobalStrideServices.AddService<IGraphicsDeviceService>(new GraphicsDeviceServiceLocal(null));

        return services;
    }
}
