using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Dependencies
{
    /// <summary>
    /// Provides a simple and potentially complex implementation of a service locator (anti-)pattern.
    /// 
    /// Simple: 
    ///  - use Get() or TryGet(createIfMissing = true), 
    ///  - Don't bother:
    ///    - changing DependencyLocatorConfiguration.UseSingletons to false
    ///    - setting up a DependencyInjection provider to build a IServiceProvider
    ///      - and then setting DependencyContext.Current.ServiceProvider = (IServiceProvider)yourProvider;
    ///      
    /// Full DependencyInjection mode:
    ///  - Turn DependencyLocatorConfiguration.UseSingletons to false
    ///  - set up a DependencyInjection provider to build a IServiceProvider
    ///    - and then set DependencyContext.Current.ServiceProvider = (IServiceProvider)yourProvider;
    /// 
    /// Hybrid with convenience (recommended / default):
    ///  - set up a DependencyInjection provider to build a IServiceProvider
    ///    - and then set DependencyContext.Current.ServiceProvider = (IServiceProvider)yourProvider;
    ///  - For concrete types you don't add to your DI container, a singleton will be instantiated upon demand.  
    ///    - (To disable, set DependencyLocatorConfiguration.AllowDefaultSingletonActivatorOnDemand to false)
    /// </summary>
    public static class DependencyLocator
    {
        public static TInterface Initialize<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface
            => Get<TInterface, TImplementation>();

        public static TInterface Get<TInterface, TImplementation>(Func<TInterface> singletonFactory = null)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            var result = TryGet<TInterface, TImplementation>(tryCreateIfMissing: true, singletonFactory: singletonFactory);
            if (result == null)
            {
                throw new Exception($"Failed to get or create non-null instance of {typeof(TInterface).Name}");
                // Should never be reached, but this could be a fallback if the GetService in TryGet fails to create
                //ManualSingleton<T>.Instance = result = new T();
            }
            return result;
        }
        public static TInterface Get<TInterface>(Func<TInterface> singletonFactory = null)
            where TInterface : class
            => Get<TInterface, TInterface>(singletonFactory: singletonFactory);


        public static TInterface TryGet<TInterface>(Func<TInterface> singletonFactory = null, bool tryCreateIfMissing = false)
        where TInterface : class
            => TryGet<TInterface, TInterface>(singletonFactory: singletonFactory, tryCreateIfMissing: tryCreateIfMissing);

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TInterface"></typeparam>
            /// <param name="singletonFactory"></param>
            /// <param name="tryCreateIfMissing">Will only be attempted if DependencyLocatorConfiguration.UseSingletons is true</param>
            /// <returns></returns>
        public static TInterface TryGet<TInterface, TImplementation>(Func<TInterface> singletonFactory = null, bool tryCreateIfMissing = false)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            TInterface result;

            if (DependencyLocatorConfiguration.UseDependencyContext)
            {
                result = DependencyContext.Current?.GetService<TInterface>();
                if (result != null) return result;
            }

            if (DependencyLocatorConfiguration.UseSingletons)
            {
                var inst = ManualSingleton<TInterface>.Instance;
                if (inst != null) return inst;

                if (tryCreateIfMissing)
                {
                    if (singletonFactory != null)
                    {
                        result = singletonFactory();
                        ManualSingleton<TInterface>.Instance = result;
                        return result;
                    }
                    else if (!typeof(TImplementation).IsInterface && DependencyLocatorConfiguration.AllowDefaultSingletonActivatorOnDemand)
                    {
                        result = Activator.CreateInstance<TImplementation>();
                        ManualSingleton<TInterface>.Instance = result;
                        return result;
                    }
                }
            }

            if (DependencyLocatorConfiguration.UseIServiceProviderSingleton)
            {
                var sp = ManualSingleton<IServiceProvider>.Instance;
                result = (TInterface)sp?.GetService(typeof(TInterface));
                if (result != null) return result;
            }

            return default;
        }

        // FUTURE: Make the Set/Get methods extensible.  Need to have type as a parameter?
        //public Func<object> GetMethod { get; set; } = () =>
        //    {
        //        var inst = ManualSingleton<T>.Instance;
        //        if (inst != null) return inst;

        //        var sp = ManualSingleton<IServiceProvider>.Instance;
        //        return (T)sp?.GetService(typeof(T));
        //    };

        //public Action<object> SetMethod { get; set; } = () =>
        //   {
        //       if (ManualSingleton<T>.Instance != null) throw new InvalidOperationException("Already set");
        //       ManualSingleton<T>.Instance = obj;
        //   };

        public static void Set<T>(T obj)
                        where T : class
        {
            if (!DependencyLocatorConfiguration.UseSingletons) { throw new Exception("Cannot use Set when SingletonConfiguration.UseSingletons == false"); }

            if (ManualSingleton<T>.Instance != null) throw new InvalidOperationException("Already set");

            ManualSingleton<T>.Instance = obj;
        }
    }
}
