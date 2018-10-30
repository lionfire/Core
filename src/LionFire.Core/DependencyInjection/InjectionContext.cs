using System;
using System.Reflection;
using System.Threading;
using LionFire.Structures;

namespace LionFire.DependencyInjection
{

    /// <summary>
    /// Wraps a IServiceProvider, with potential fallback to other IServiceProviders
    /// </summary>
    /// <remarks>
    /// FUTURE:
    ///  - Named services?
    /// </remarks>
    public class InjectionContext : IServiceProvider
    {
        public static void Reset()
        {
            current = null;
            Default = null;
        }

        #region (Static)

        /// <summary>
        /// REVIEW - need to think seriously about this.
        ///  - AsyncLocal
        ///  - Thread local / threadstatic
        ///  - Ambient stack (maybe also async/thread local)
        /// </summary>
        public static InjectionContext Current
        {
            get => AsyncLocal ?? current ?? Default;
            set
            {
                if (current != null && value != null && value != current)
                {
                    throw new Exception("Cannot be set to another value without first setting to null.");
                }
                current = value;
            }
        }
        protected static InjectionContext current;

        /// <summary>
        /// Set by AppHost
        /// </summary>
        public static InjectionContext Default
        {
            get => ManualSingleton<InjectionContext>.Instance;
            set
            {
                if (value == ManualSingleton<InjectionContext>.Instance)
                {
                    return;
                }

                if (value != null && ManualSingleton<InjectionContext>.Instance != null)
                {
                    throw new AlreadyException("InjectionContext.Default is already set");
                }
                ManualSingleton<InjectionContext>.Instance = value;
            }
        }

        //public static void UseDefaultServiceProvider()
        //{
        //    if (ManualSingleton<IServiceProvider>.Instance != null)
        //    {
        //        throw new Exception("ManualSingleton<IServiceProvider>.Instance is already set");
        //    }
        //    ManualSingleton<IServiceProvider>.Instance = Current;
        //}

        #endregion

        //public void UseSingletonInstance<T>(T singletonInstance)
        //    where T : class
        //{
        //    ManualSingleton<T>.Instance = singletonInstance;
        //}

        #region ServiceProvider

        public IServiceProvider ServiceProvider
        {
            get => serviceProvider;
            set => serviceProvider = value;
        }
        private IServiceProvider serviceProvider;

        #endregion

        #region AsyncLocal

        public static InjectionContext AsyncLocal
        {
            get => asyncLocal?.Value;
            set
            {
                if (asyncLocal == null)
                {
                    asyncLocal = new AsyncLocal<InjectionContext>();
                }

                asyncLocal.Value = value;
            }
        }
        private static AsyncLocal<InjectionContext> asyncLocal;

        #endregion

        #region ThreadLocal

        public static InjectionContext ThreadLocal
        {
            get => threadLocal?.Value;
            set
            {
                if (threadLocal == null)
                {
                    threadLocal = new ThreadLocal<InjectionContext>();
                }

                threadLocal.Value = value;
            }
        }
        private static ThreadLocal<InjectionContext> threadLocal;

        #endregion




        public T GetService<T>(IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing)
        {
            var mi = typeof(InjectionContext).GetMethod("GetService", new Type[] { typeof(Type), typeof(IServiceProvider), typeof(bool) });
            return (T)mi.Invoke(this, new object[] { typeof(T), serviceProvider, createIfMissing });
        }

        //private readonly bool UseManualSingletonServiceProvider = false;

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

            #region Try IServiceProvider from Parameter

            if (serviceProvider != null && serviceProvider != this)
            {
                result = serviceProvider.GetService(serviceType);
                if (result != null) { return result; }
            }

            #endregion

            #region Try this.ServiceProvider
            {
                var _serviceProvider = ServiceProvider;
                if (_serviceProvider != null)
                {
                    result = _serviceProvider.GetService(serviceType);
                    if (result != null) { return result; }
                }
            }
            #endregion

            //#region Try ManualSingleton<IServiceProvider>.Instance

            //if (UseManualSingletonServiceProvider)
            //{
            //    var _serviceProvider = ManualSingleton<IServiceProvider>.Instance;
            //    if (_serviceProvider != null && _serviceProvider != this)
            //    {
            //        result = _serviceProvider.GetService(serviceType);
            //        if (result != null) { return result; }
            //    }
            //}

            //#endregion

            #region Try ManualSingleton<>'s GuaranteedInstance (if createIfMissing is true), or else Instance

            if (!serviceType.GetTypeInfo().IsInterface)
            {
                // BREAKINGCHANGE TODO: store the instance with this object, rather than the global ManualSingleton<>.Instance

                var pi = typeof(ManualSingleton<>).MakeGenericType(serviceType).GetProperty(createIfMissing ? "GuaranteedInstance" : "Instance", BindingFlags.Static | BindingFlags.Public);
                result = pi.GetValue(null); // Might be null for ManualSingleton<>.Instance
                if (result != null) { return result; }
            }
            #endregion

            return null;
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

        //public void AddSingleton<T>(T obj, bool force = false)
        //    where T : class
        //{
        //    if (!force && ManualSingleton<T>.Instance != null && !object.ReferenceEquals(ManualSingleton<T>.Instance, obj))
        //    {
        //        throw new AlreadyException($"{typeof(T).Name} singleton already set.  Use force to override.");
        //    }
        //    ManualSingleton<T>.Instance = obj;
        //}

        object IServiceProvider.GetService(Type serviceType) => GetService(serviceType, null, DefaultCreateIfMissing);
        private const bool DefaultCreateIfMissing = true;

    }

}
