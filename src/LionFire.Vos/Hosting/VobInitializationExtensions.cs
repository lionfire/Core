using System;
using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Dependencies;
using System.Collections.Generic;

namespace LionFire.Services
{
    public static class VobInitializationExtensions
    {
        //public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IVob> action)
        //{
        //    services.TryAddEnumerableSingleton(new VobInitializer(action));
        //    return services;
        //}
        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobPath, Action<IVob> action)
        //{
        //    services.TryAddEnumerableSingleton(new VobInitializer(action) { VobPath = vobPath });
        //    return services;
        //}
        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobRootName, string vobPath, Action<IVob> action)
        //{
        //    services.TryAddEnumerableSingleton(new VobInitializer(action) { VobPath = vobPath, VobRootName = vobRootName });
        //    return services;
        //}

        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IServiceProvider, IVob> action)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(action)));
            return services;
        }
        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IVob> action)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(action)));
            return services;
        }
        public static IServiceCollection InitializeVob(this IServiceCollection services, string vobPath, Action<IVob> action)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(action) { VobPath = vobPath }));
            return services;
        }
        public static IServiceCollection InitializeVob(this IServiceCollection services, string vobRootName, string vobPath, Action<IVob> action)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(action) { VobPath = vobPath, VobRootName = vobRootName }));
            return services;
        }

        public static IServiceCollection InitializeVob(this IServiceCollection services, IEnumerable<string> vobPath, Action<IVob> action)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(action) { VobPathChunks = vobPath }));
            return services;
        }
    }
}


