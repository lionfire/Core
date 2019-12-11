using LionFire.Reflection;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionFire.Dependencies
{
    /// <summary>
    /// Provides a simple and potentially complex implementation of a service locator (anti-)pattern.
    /// 
    /// Simple: 
    ///  - use Get() or TryGet(createIfMissing = true),  -- will create a singleton instance
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

        #region Get

        public static TInterface Get<TInterface, TImplementation>(Func<TInterface> singletonFactory = null)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            var result = TryGet<TInterface, TImplementation>(tryCreateIfMissing: true, singletonFactory: singletonFactory);
            if (result == null)
            {
                throw new DependencyMissingException($"Failed to get or create non-null instance of {typeof(TInterface).Name}");
                // Should never be reached, but this could be a fallback if the GetService in TryGet fails to create
                //ManualSingleton<T>.Instance = result = new T();
            }
            return result;
        }
        public static TInterface Get<TInterface>(Func<TInterface> singletonFactory = null)
            where TInterface : class
            => Get<TInterface, TInterface>(singletonFactory: singletonFactory);

        public static TReturnValue Get<TReturnValue>(Type interfaceType)
            where TReturnValue : class
            => (TReturnValue)Get_TInterface_Func.MakeGenericMethod(interfaceType).Invoke(null, new object[] { null });
            //=> (TInterface)GetMethodEx.GetMethodExt(typeof(DependencyLocator), nameof(Get), BindingFlags.Static | BindingFlags.Public, typeof(Func<>)).Invoke(null,  new object[] { singletonFactory });
        //=> (TInterface)GetMethodEx.GetMethodExt(typeof(DependencyLocator), nameof(Get), BindingFlags.Static | BindingFlags.Public, typeof(Func<TInterface>)).Invoke(null,  new object[] { singletonFactory });
        //=> typeof(DependencyLocator).GetMethod(nameof(Get), Get<TInterface, TInterface>(singletonFactory: singletonFactory);

        private static MethodInfo Get_TInterface_Func;

        static DependencyLocator()
        {
            Get_TInterface_Func = typeof(DependencyLocator).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mi => mi.ContainsGenericParameters && mi.GetGenericArguments().Length == 1).First();
        }

        #endregion

        #region TryGet

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
            TInterface result = default;

            IServiceProvider serviceProvider = default;

            IServiceProvider GetServiceProvider()
            {
                if (serviceProvider != null) return serviceProvider;
                var spResult = DependencyContext.Current?.ServiceProvider;
                if (spResult != null)
                {
                    serviceProvider = spResult;
                    return spResult;
                }

                //if (DependencyLocatorConfiguration.UseIServiceProviderSingleton)
                //{
                //    spResult = ManualSingleton<IServiceProvider>.Instance;
                //    if (spResult != null)
                //    {
                //        serviceProvider = spResult;
                //        return spResult;
                //    }
                //}
                return null;
            }

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
                    else if (!typeof(TImplementation).IsInterface)
                    {
                        if (DependencyLocatorConfiguration.UseServiceProviderToActivateSingletons && (serviceProvider ?? GetServiceProvider()) != null)
                        {
                            return ManualSingleton<TInterface>.Instance = ActivatorUtilities.CreateInstance<TImplementation>(serviceProvider);
                        }
                        else if (DependencyLocatorConfiguration.AllowDefaultSingletonActivatorOnDemand)
                        {
                            return ManualSingleton<TInterface>.Instance = Activator.CreateInstance<TImplementation>();
                        }
                    }
                }
            }

            //if (DependencyLocatorConfiguration.UseIServiceProviderSingleton)
            //{
            //    var sp = ManualSingleton<IServiceProvider>.Instance;
            //    result = (TInterface)sp?.GetService(typeof(TInterface));
            //    if (result != null) return result;
            //}

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

        #endregion

        #region Set

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

        #endregion

    }
}

namespace LionFire.Reflection
{
    // From https://stackoverflow.com/a/7182379/208304
    public static class GetMethodEx
    {
        /// <summary>
        /// Search for a method by name and parameter types.  
        /// Unlike GetMethod(), does 'loose' matching on generic
        /// parameter types, and searches base interfaces.
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static MethodInfo GetMethodExt(this Type thisType,
                                                string name,
                                                params Type[] parameterTypes)
        {
            return GetMethodExt(thisType,
                                name,
                                BindingFlags.Instance
                                | BindingFlags.Static
                                | BindingFlags.Public
                                | BindingFlags.NonPublic
                                | BindingFlags.FlattenHierarchy,
                                parameterTypes);
        }

        /// <summary>
        /// Search for a method by name, parameter types, and binding flags.  
        /// Unlike GetMethod(), does 'loose' matching on generic
        /// parameter types, and searches base interfaces.
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static MethodInfo GetMethodExt(this Type thisType,
                                                string name,
                                                BindingFlags bindingFlags,
                                                params Type[] parameterTypes)
        {
            MethodInfo matchingMethod = null;

            // Check all methods with the specified name, including in base classes
            GetMethodExt(ref matchingMethod, thisType, name, bindingFlags, parameterTypes);

            // If we're searching an interface, we have to manually search base interfaces
            if (matchingMethod == null && thisType.IsInterface)
            {
                foreach (Type interfaceType in thisType.GetInterfaces())
                    GetMethodExt(ref matchingMethod,
                                 interfaceType,
                                 name,
                                 bindingFlags,
                                 parameterTypes);
            }

            return matchingMethod;
        }

        private static void GetMethodExt(ref MethodInfo matchingMethod,
                                            Type type,
                                            string name,
                                            BindingFlags bindingFlags,
                                            params Type[] parameterTypes)
        {
            // Check all methods with the specified name, including in base classes
            foreach (MethodInfo methodInfo in type.GetMember(name,
                                                             MemberTypes.Method,
                                                             bindingFlags))
            {
                // Check that the parameter counts and types match, 
                // with 'loose' matching on generic parameters
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                if (parameterInfos.Length == parameterTypes.Length)
                {
                    int i = 0;
                    for (; i < parameterInfos.Length; ++i)
                    {
                        if (!parameterInfos[i].ParameterType
                                              .IsSimilarType(parameterTypes[i]))
                            break;
                    }
                    if (i == parameterInfos.Length)
                    {
                        if (matchingMethod == null)
                            matchingMethod = methodInfo;
                        else
                            throw new AmbiguousMatchException(
                                   "More than one matching method found!");
                    }
                }
            }
        }

        /// <summary>
        /// Special type used to match any generic parameter type in GetMethodExt().
        /// </summary>
        public class T
        { }

        /// <summary>
        /// Determines if the two types are either identical, or are both generic 
        /// parameters or generic types with generic parameters in the same
        ///  locations (generic parameters match any other generic paramter,
        /// but NOT concrete types).
        /// </summary>
        private static bool IsSimilarType(this Type thisType, Type type)
        {
            // Ignore any 'ref' types
            if (thisType.IsByRef)
                thisType = thisType.GetElementType();
            if (type.IsByRef)
                type = type.GetElementType();

            // Handle array types
            if (thisType.IsArray && type.IsArray)
                return thisType.GetElementType().IsSimilarType(type.GetElementType());

            // If the types are identical, or they're both generic parameters 
            // or the special 'T' type, treat as a match
            if (thisType == type || ((thisType.IsGenericParameter || thisType == typeof(T))
                                 && (type.IsGenericParameter || type == typeof(T))))
                return true;

            // Handle any generic arguments
            if (thisType.IsGenericType && type.IsGenericType)
            {
                Type[] thisArguments = thisType.GetGenericArguments();
                Type[] arguments = type.GetGenericArguments();
                if (thisArguments.Length == arguments.Length)
                {
                    for (int i = 0; i < thisArguments.Length; ++i)
                    {
                        if (!thisArguments[i].IsSimilarType(arguments[i]))
                            return false;
                    }
                    return true;
                }
            }

            return false;
        }
    }
}