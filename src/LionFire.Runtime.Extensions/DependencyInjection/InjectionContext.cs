using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using LionFire.Structures;

namespace LionFire.DependencyInjection
{


    public class InjectionContext : IServiceProvider
    {
        #region Static

        // FUTURE: Facilitate per-thread context?  An ambient stack using Disposable?
        public static InjectionContext Current { get { return current ?? Default; } set { current = value; } }
        public static InjectionContext current;
        public static InjectionContext Default { get { return ManualSingleton<InjectionContext>.GuaranteedInstance; } }

        static InjectionContext()
        {
            ManualSingleton<IServiceProvider>.SetIfMissing(Current);
        }

        public static void SetSingletonDefault<T>(T singletonInstance)
            where T : class
        {
            ManualSingleton<T>.Instance = singletonInstance;
        }

        #endregion


        #region ServiceProvider

        public IServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
            set { serviceProvider = value; }
        }
        private IServiceProvider serviceProvider;

        #endregion

        public T GetService<T>(IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing)
        {
            var mi = typeof(InjectionContext).GetMethod("GetService", new Type[] { typeof(Type), typeof(IServiceProvider), typeof(bool) });
            return (T)mi.Invoke(this, new object[] { typeof(T), serviceProvider, createIfMissing });
        }

        /// <summary>
        /// Locate the service for the specified type.
        /// Search order:
        ///  - serviceProvider.GetService() parameter (if specified)
        ///  - ManualSingleton&lt;IServiceProvider&gt;.Instance.GetService(), which is set by the root AppHost
        ///  - createIfMissing:
        ///    - true: ManualSingleton&lt;IServiceProvider&gt;.GuaranteedInstance
        ///    - false: ManualSingleton&lt;IServiceProvider&gt;.Instance
        /// </summary>
        /// <param name="serviceType">Type of service to locate</param>
        /// <param name="serviceProvider">ServiceProvider to try first.  If not found, alternatives will be attempted.</param>
        /// <param name="createIfMissing">If true, will be created at ManualSingleton&lt;IServiceProvider&gt;.GuaranteedInstance if not found anywhere else</param>
        /// <returns></returns>
        public virtual object GetService(Type serviceType, IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing)
        {
            object result;

            if (serviceProvider != null && serviceProvider != this)
            {
                result = serviceProvider.GetService(serviceType);
                if (result != null) { return result; }
            }

            serviceProvider = ManualSingleton<IServiceProvider>.Instance;
            if (serviceProvider != null && serviceProvider != this)
            {
                result = serviceProvider.GetService(serviceType);
                if (result != null) { return result; }
            }

            serviceProvider = this.ServiceProvider;
            if (serviceProvider != null)
            {
                result = serviceProvider.GetService(serviceType);
                if (result != null) { return result; }
            }

            var pi = typeof(ManualSingleton<>).MakeGenericType(serviceType).GetProperty(createIfMissing ? "GuaranteedInstance" : "Instance", BindingFlags.Static | BindingFlags.Public);
            result = pi.GetValue(null);

            return result;
        }

        public void AddSingleton<T>(T obj, bool force = false)
            where T : class
        {
            if (!force && ManualSingleton<T>.Instance != null && !object.ReferenceEquals(ManualSingleton<T>.Instance, obj))
            {
                throw new AlreadyException($"{typeof(T).Name} singleton already set.  Use force to override.");
            }
            ManualSingleton<T>.Instance = obj;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return GetService(serviceType, null, DefaultCreateIfMissing);
        }
        private const bool DefaultCreateIfMissing = true;

    }

}
