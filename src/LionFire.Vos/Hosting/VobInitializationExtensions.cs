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



        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IServiceProvider, IVob> action, string rootName = VosConstants.DefaultRootName)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(VosReference.FromRootName(rootName), action)));
            return services;
        }

        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IVob> action, string rootName = VosConstants.DefaultRootName)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(VosReference.FromRootName(rootName), action)));
            return services;
        }

        #region InitializeVob

        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobPath, Action<IServiceProvider, IVob> action) 
        //=> services.InitializeVob(vobPath.ToVosReference(), action);

        public static IServiceCollection InitializeVob(this IServiceCollection services, IVosReference vob, Action<IServiceProvider, IVob> action)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(vob, action)));
            return services;
        }

        public static IServiceCollection InitializeVob(this IServiceCollection services, VosReference vobPath, Action<IVob> action)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(vobPath.ToVosReference(), action)));
            return services;
        }
        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobRootName, string vobPath, Action<IVob> action)
        //{
        //    services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(vobPath.ToVosReference(), action) { VobPath = vobPath, VobRootName = vobRootName }));
        //    return services;
        //}

        public static IServiceCollection InitializeVob(this IServiceCollection services, IEnumerable<string> vobPath, Action<IVob> action)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(new VosReference(vobPath), action)));
            return services;
        }

        #endregion

    }
}


