using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using LionFire.Structures;

namespace LionFire.Dependencies
{

    public static class DependencyContextExtensions
    {
    }

    /// <summary>
    /// Wraps a IServiceProvider, with potential fallback to other IServiceProviders
    /// </summary>
    /// <remarks>
    /// FUTURE:
    ///  - Named services?
    /// </remarks>
    public class DependencyContext : IServiceProvider // RENAME to AmbientContext?
    {
        public static void Reset()
        {
            current = null;
            Default = null;
        }

        public void UseAsGuaranteedSingletonProvider(bool useDefaultAsFallback = true)
        {
            ManualSingletonProvider.GuaranteedInstanceProvider =
                GuaranteedInstanceProvider(fallback: useDefaultAsFallback ? ManualSingletonProvider.GuaranteedInstanceProvider : null);
        }

        protected Func<Type, object> GuaranteedInstanceProvider(Func<Type, object> fallback) =>
            new Func<Type, object>(createType => GetService(createType) ?? fallback?.Invoke(createType));

        #region (Static)

        /// <summary>
        /// REVIEW - need to think seriously about this.
        ///  - AsyncLocal
        ///  - Thread local / threadstatic
        ///  - Ambient stack (maybe also async/thread local)
        /// </summary>
        public static DependencyContext Current
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
        protected static DependencyContext current;

        /// <summary>
        /// Set by AppHost
        /// </summary>
        public static DependencyContext Default
        {
            get => ManualSingleton<DependencyContext>.GuaranteedInstance;
            set
            {
                if (value == ManualSingleton<DependencyContext>.Instance)
                {
                    return;
                }

                if (
                    value != null &&
                    ManualSingleton<DependencyContext>.Instance != null)
                {
                    throw new AlreadyException("DependencyContext.Default is already set");
                }
                ManualSingleton<DependencyContext>.Instance = value;
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

        public static DependencyContext AsyncLocal
        {
            get => asyncLocal?.Value;
            set
            {
                if (asyncLocal == null)
                {
                    asyncLocal = new AsyncLocal<DependencyContext>();
                }

                asyncLocal.Value = value;
            }
        }
        private static AsyncLocal<DependencyContext> asyncLocal;

        #endregion

        #region ThreadLocal

        public static DependencyContext ThreadLocal
        {
            get => threadLocal?.Value;
            set
            {
                if (threadLocal == null)
                {
                    threadLocal = new ThreadLocal<DependencyContext>();
                }

                threadLocal.Value = value;
            }
        }
        private static ThreadLocal<DependencyContext> threadLocal;

        #endregion

#nullable enable
        public T? GetService<T>(IServiceProvider? serviceProvider = null) where T : class => (T)GetService(typeof(T), serviceProvider);
        public T GetRequiredService<T>(IServiceProvider? serviceProvider = null) => (T)GetService(typeof(T), serviceProvider) ?? throw new DependencyMissingException(typeof(T).FullName);

        public IEnumerable<T>? GetServices<T>(IServiceProvider? serviceProvider = null)
            where T : class
        {
            var singleResult = GetService<T>(serviceProvider);
            return singleResult != null ? (new T[] { singleResult }) : GetService<IEnumerable<T>>();
        }
#nullable disable


        //public TInterface GetServiceOrSingleton<TInterface>(IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing, Func<TInterface> singletonFactory = null)
        //=> (TInterface)GetServiceOrSingleton(typeof(TInterface), serviceProvider, createIfMissing, singletonFactory: singletonFactory);

        public TInterface GetServiceOrSingleton<TInterface, TConcrete>(IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing, Func<TInterface> singletonFactory = null)
            => (TInterface)GetServiceOrSingleton(typeof(TInterface), serviceProvider, createIfMissing, typeof(TConcrete),
                singletonFactory: (singletonFactory != null ? () => singletonFactory() : (Func<object>)null));

        public TInterface GetServiceOrSingleton<TInterface>(IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing, Func<TInterface> singletonFactory = null)
            => (TInterface)GetServiceOrSingleton(typeof(TInterface), serviceProvider, createIfMissing, concreteType: null,
                singletonFactory: (singletonFactory != null ? () => singletonFactory() : (Func<object>)null));

        //private readonly bool UseManualSingletonServiceProvider = false;

        public virtual object GetService(Type serviceType, IServiceProvider serviceProvider = null)
        {
            object result = null;

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

            return result;
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
        /// <param name="interfaceType">Type of service interface to locate</param>
        /// <param name="serviceProvider">ServiceProvider to try first.  If not found, alternatives will be attempted.</param>
        /// <param name="createIfMissing">If true, will be created at ManualSingleton&lt;IServiceProvider&gt;.GuaranteedInstance if not found anywhere else.  If false, and ManualSingleton.Instance is null, null will be returned (after trying GetService).</param>
        /// <returns></returns>
        /// <remarks>
        /// REVIEW for performance
        /// </remarks>
        public virtual object GetServiceOrSingleton(Type interfaceType, IServiceProvider serviceProvider = null, bool createIfMissing = true, Type concreteType = null, Func<object> singletonFactory = null)
        {
            object result = GetService(interfaceType, serviceProvider);

            if (result != null) return result;

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

            // Try ManualSingleton<>'s GuaranteedInstance (if createIfMissing is true), or else Instance

            if (concreteType != null)
            {
                // BREAKINGCHANGE TODO: store the instance with this object, rather than the global ManualSingleton<>.Instance

                var pi = typeof(ManualSingleton<>).MakeGenericType(concreteType).GetProperty(createIfMissing ? "GuaranteedInstance" : "Instance", BindingFlags.Static | BindingFlags.Public);
                result = pi.GetValue(null); // Might be null for ManualSingleton<>.Instance
                if (result != null) { return result; }
            }

            var canCreate = !interfaceType.IsInterface && !interfaceType.IsAbstract;

            // BREAKINGCHANGE TODO: store the instance with this object, rather than the global ManualSingleton<>.Instance
            {
                var pi = typeof(ManualSingleton<>).MakeGenericType(interfaceType).GetProperty(createIfMissing && canCreate ? "GuaranteedInstance" : "Instance", BindingFlags.Static | BindingFlags.Public);
                result = pi.GetValue(null); // Might be null for ManualSingleton<>.Instance
                return result;
            }
        }


        //public IEnumerable<T> GetServices<T>(IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing)
        //{
        //    var mi = typeof(DependencyContext).GetMethod("GetServices", new Type[] { typeof(Type), typeof(IServiceProvider), typeof(bool) });
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

        object IServiceProvider.GetService(Type serviceType) => GetService(serviceType, null);
        public object GetServiceOrSingleton(Type serviceType) => GetServiceOrSingleton(serviceType, null, DefaultCreateIfMissing);
        private const bool DefaultCreateIfMissing = true;

    }

}
