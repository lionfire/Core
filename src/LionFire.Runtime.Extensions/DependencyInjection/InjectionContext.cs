using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.DependencyInjection
{


    public class InjectionContext : IServiceProvider
    {
        #region Static

        // FUTURE: Facilitate per-thread context?  An ambient stack using Disposable?
        public static InjectionContext Current
        {
            get { return current ?? Default; }
            set
            {
                if (current != null && value != null && value != current)
                {
                    throw new Exception("Cannot be set to another value without first setting to null.");
                }
                current = value;
            }
        }
        public static InjectionContext current;
        public static InjectionContext Default { get { return ManualSingleton<InjectionContext>.Instance; } set { ManualSingleton<InjectionContext>.Instance = value; } }

        static InjectionContext()
        {
        }

        public static void UseDefaultServiceProvider()
        {
            if (ManualSingleton<IServiceProvider>.Instance != null)
            {
                throw new Exception("ManualSingleton<IServiceProvider>.Instance is already set");
            }
            ManualSingleton<IServiceProvider>.Instance = Current;
        }

        #endregion

        //public void UseSingletonInstance<T>(T singletonInstance)
        //    where T : class
        //{
        //    ManualSingleton<T>.Instance = singletonInstance;
        //}

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



        private bool UseManualSingletonServiceProvider = false;

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

            {
                var _serviceProvider = this.ServiceProvider;
                if (_serviceProvider != null)
                {
                    result = _serviceProvider.GetService(serviceType);
                    if (result != null) { return result; }
                }
            }

            if (UseManualSingletonServiceProvider)
            {
                var _serviceProvider = ManualSingleton<IServiceProvider>.Instance;
                if (_serviceProvider != null && _serviceProvider != this)
                {
                    result = _serviceProvider.GetService(serviceType);
                    if (result != null) { return result; }
                }
            }

            var pi = typeof(ManualSingleton<>).MakeGenericType(serviceType).GetProperty(createIfMissing ? "GuaranteedInstance" : "Instance", BindingFlags.Static | BindingFlags.Public);
            result = pi.GetValue(null);

            return result;
        }

        //public IEnumerable<T> GetServices<T>(IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing)
        //{
        //    var mi = typeof(InjectionContext).GetMethod("GetServices", new Type[] { typeof(Type), typeof(IServiceProvider), typeof(bool) });
        //    return (IEnumerable<T>)mi.Invoke(this, new object[] { typeof(T), serviceProvider, createIfMissing });
        //}

        //public virtual IEnumerable<object> GetServices(Type serviceType, IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing)
        //{
        //    IEnumerable<object> result;

        //    if (serviceProvider != null && serviceProvider != this)
        //    {
        //        result = serviceProvider.GetServices(serviceType);
        //        if (result != null) { return result; }
        //    }

        //    {
        //        var _serviceProvider = this.ServiceProvider;
        //        if (_serviceProvider != null)
        //        {
        //            result = _serviceProvider.GetServices(serviceType);
        //            if (result != null) { return result; }
        //        }
        //    }

        //    if (UseManualSingletonServiceProvider)
        //    {
        //        var _serviceProvider = ManualSingleton<IServiceProvider>.Instance;
        //        if (_serviceProvider != null && _serviceProvider != this)
        //        {
        //            result = _serviceProvider.GetServices(serviceType);
        //            if (result != null) { return result; }
        //        }
        //    }

        //    var pi = typeof(ManualSingleton<>).MakeGenericType(serviceType).GetProperty(createIfMissing ? "GuaranteedInstance" : "Instance", BindingFlags.Static | BindingFlags.Public);
        //    var service = pi.GetValue(null);
        //    result = service == null ? Enumerable.Empty<object>() : new object[] { service };

        //    return result;
        //}

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
