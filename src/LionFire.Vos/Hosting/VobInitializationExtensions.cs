#nullable enable
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



        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IServiceProvider, IRootVob> action, string rootName = VosConstants.DefaultRootName, IEnumerable<string>? prerequisites = null, IEnumerable<string>? provides = null)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(VosReference.FromRootName(rootName), action) { Prerequisites = prerequisites, Provides = provides }));
            return services;
        }

        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IRootVob> action, string rootName = VosConstants.DefaultRootName, IEnumerable<string>? prerequisites = null, IEnumerable<string>? provides = null)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(VosReference.FromRootName(rootName), action) { Prerequisites = prerequisites, Provides = provides }));
            return services;
        }

        #region InitializeVob

        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobPath, Action<IServiceProvider, IVob> action) 
        //=> services.InitializeVob(vobPath.ToVosReference(), action);

        public static IServiceCollection InitializeVob(this IServiceCollection services, IVosReference vob, Action<IServiceProvider, IVob> action, IEnumerable<string>? prerequisites = null, IEnumerable<string>? provides = null)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(vob, action) { Prerequisites = prerequisites, Provides = provides }));
            return services;
        }

        public static IServiceCollection InitializeVob(this IServiceCollection services, VosReference vobPath, Action<IVob> action, IEnumerable<string>? prerequisites = null, IEnumerable<string>? provides = null)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(vobPath.ToVosReference(), action) { Prerequisites = prerequisites, Provides = provides }));
            return services;
        }
        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobRootName, string vobPath, Action<IVob> action)
        //{
        //    services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(vobPath.ToVosReference(), action) { VobPath = vobPath, VobRootName = vobRootName }));
        //    return services;
        //}

        public static IServiceCollection InitializeVob(this IServiceCollection services, IEnumerable<string> vobPath, Action<IVob> action, IEnumerable<string>? prerequisites = null, IEnumerable<string>? provides = null)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(new VosReference(vobPath), action) { Prerequisites = prerequisites, Provides = provides }));
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="vobPath"></param>
        /// <param name="action">Return true if completed successfully, false if the action should be invoked again after trying other initializers.</param>
        /// <returns></returns>
        public static IServiceCollection InitializeVob(this IServiceCollection services, VosReference vobPath, Func<IServiceProvider, IVob, object> action, IEnumerable<string>? prerequisites = null, IEnumerable<string>? provides = null)
        {
            services.Configure<List<VobInitializer>>(list => list.Add(new VobInitializer(vobPath.ToVosReference(), action) { Prerequisites = prerequisites, Provides = provides }));
            return services;
        }

        #endregion

    }
}


