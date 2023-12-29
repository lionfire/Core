using Microsoft.Extensions.DependencyInjection;
using Stride.Core.IO;
using Stride.Core.Serialization.Contents;
using Stride.Core.Storage;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Content manager
        var Content = new ContentManager(GlobalStrideServices);
        services.AddSingleton<ContentManager>(Content);
        GlobalStrideServices.AddService<IContentManager>(Content);
        GlobalStrideServices.AddService(Content);

        //GlobalStrideServices.AddService<IGraphicsDeviceService>(new GraphicsDeviceServiceLocal(null));

        return services;
    }
}
