#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using LionFire.ExtensionMethods;
using LionFire.MultiTyping;
using LionFire.Ontology;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Dependencies;

// ENH idea: during construction of IHostBuilder or HostApplicationBuilder, register builder with DependencyContext, and allow GetService which will get IServiceProvider from IServiceCollection so far

// REVIEW - this wraps ServiceProvider using 4 interfaces. Superfluous?
public interface IDependencyContext : IHas<IServiceProvider?>, IWrapper<IServiceProvider?>, IServiceProvider // RENAME to IServiceProviderContext?
{
    IServiceProvider? ServiceProvider { get; }
}

/// <summary>
/// Wraps a IServiceProvider, with potential fallback to other IServiceProviders
/// </summary>
/// <remarks>
/// RENAME to ServiceProviderContext?
/// FUTURE:
///  - Named services?
/// </remarks>
public class DependencyContext : IDependencyContext
{
    #region (static)
    // TODO FIXME - figure out how to initialize Default and Current and when to use each

    /// <summary>
    /// Initializes DependencyContext.Current, or the DependencyContext.AsyncLocal  if LionFireEnvironment.IsMultiApplicationEnvironment is true
    /// </summary>
    public static void InitializeCurrent()
    {
        if (LionFireEnvironment.IsMultiApplicationEnvironment)
        {
            DependencyLocatorConfiguration.UseServiceProviderToActivateSingletons = false;
            DependencyLocatorConfiguration.UseSingletons = false;

            if (DependencyContext.AsyncLocal != null) throw new AlreadyException("UNEXPECTED: LionFireEnvironment.IsMultiApplicationEnvironment == true && DependencyContext.AsyncLocal != null "); // FUTURE - deinit on Run complete?  Unit tests running in series?

            DependencyContext.AsyncLocal = new DependencyContext();
        }
        else
        {
            if (DependencyContext.Current == null)
            {
                DependencyContext.Current = new DependencyContext();
            }
        }
    }

    // TODO - this is not called
    // Caution: could be race condition at startup where this may not be set yet.  Hook into IHostLifetime.WaitForStartAsync?
    public static IServiceProvider RegisterServiceProvider(IServiceProvider serviceProvider)
    {
        if (LionFireEnvironment.IsMultiApplicationEnvironment)
        {
            if (DependencyContext.AsyncLocal == null) throw new Exception("IsMultiApplicationEnvironment but DependencyContext.AsyncLocal == null.  Call InitializeCurrent() first.");
            DependencyContext.AsyncLocal.ServiceProvider = serviceProvider;
        }
        else
        {
            DependencyContext.Current ??= new DependencyContext();

            if (DependencyContext.Current.ServiceProvider == null)
            {
                DependencyContext.Current.ServiceProvider = serviceProvider;
            }
            else if (!ReferenceEquals(serviceProvider, DependencyContext.Current.ServiceProvider)) { throw new AlreadySetException(); }
        }
        return serviceProvider;
    }

    //[Obsolete("Use Deinitialize()")]
    //public static void Reset()
    //{
    //    Deinitialize();
    //}
    public static void Deinitialize()
    {
        if (AsyncLocal != null) // REVIEW
        {
            Deinitialize(AsyncLocal);
            return;
        }

        AsyncLocal = null;
        Default = null;

        var current = Current;
        if (current != null)
        {
            current.ServiceProvider = null;
            Current = null;
        }
    }
    public static void Deinitialize(DependencyContext dependencyContext)
    {

    }

    /// <summary>
    /// REVIEW - need to think seriously about this.
    ///  - AsyncLocal
    ///  - Thread local / threadstatic
    ///  - Ambient stack (maybe also async/thread local)
    /// </summary>
    public static DependencyContext? Current
    {
        get
        {
            if (LionFireEnvironment.IsMultiApplicationEnvironment)
            {
                return AsyncLocal;
            }
            else
            {
                return AsyncLocal ?? current ?? Default;
            }
        }
        set
        {
            if (LionFireEnvironment.IsMultiApplicationEnvironment)
            {
                AsyncLocal = value;
            }
            else
            {
                if (current != null && value != null && value != current)
                {
                    throw new Exception("Cannot be set to another value without first setting to null.");
                }
                current = value;
            }
        }
    }
    protected static DependencyContext? current;

    /// <summary>
    /// Set by AppHost
    /// </summary>
    public static DependencyContext? Default
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

    #region AsyncLocal

    public static DependencyContext? AsyncLocal
    {
        get => asyncLocal?.Value;
        set
        {
            asyncLocal ??= new AsyncLocal<DependencyContext?>();
            asyncLocal.Value = value;
        }
    }
    private static AsyncLocal<DependencyContext?> asyncLocal;

    #endregion

    #region ThreadLocal

    public static DependencyContext? ThreadLocal
    {
        get => threadLocal?.Value;
        set
        {
            threadLocal ??= new ThreadLocal<DependencyContext?>();
            threadLocal.Value = value;
        }
    }
    private static ThreadLocal<DependencyContext?>? threadLocal;

    public static IServiceProvider? CurrentServiceProvider => GlobalServiceProvider ?? Current?.ServiceProvider;

    public static DependencyContext? Global
    {
        get
        {
            if (!LionFireEnvironment.IsMultiApplicationEnvironment)
            {
                return global ??= new();
            }
            return null;
        }
    }
    private static DependencyContext? global;

    /// <summary>
    /// For simple/typical programs that have one root IServiceProvider, it is held here.  It is discarded upon an attempt to set it to a subsequent different value.
    /// </summary>
    public static IServiceProvider? GlobalServiceProvider // was SingleRootServiceProvider
    {
        get => Global?.ServiceProvider;
        set
        {
            if (Global == null) { throw new InvalidOperationException("Cannot set GlobalServiceProvider. Not supported when LionFireEnvironment.IsMultiApplicationEnvironment is true. "); }

            if (noGlobalServiceProviderBecauseThereAreMultiple) { return; }

            if (GlobalServiceProvider != null)
            {
                if (Object.ReferenceEquals(value, GlobalServiceProvider)) { return; }

                Global.ServiceProvider = null;
                noGlobalServiceProviderBecauseThereAreMultiple = true;
                return;
            }

            Global.ServiceProvider = value;
        }
    }
    private static volatile bool noGlobalServiceProviderBecauseThereAreMultiple;

    #endregion

    #endregion

    #region ServiceProvider

    public IServiceProvider? ServiceProvider
    {
        get => serviceProvider;
        set
        {
            if (value != null && serviceProvider != null && !ReferenceEquals(value, serviceProvider)) { throw new AlreadySetException($"{nameof(ServiceProvider)} is already set. It must be first be set back to null, to avoid unintentional overwriting."); }
            serviceProvider = value;
        }
    }
    private IServiceProvider? serviceProvider;

    IServiceProvider? IHas<IServiceProvider?>.Object => ServiceProvider;
    #region IWrapper<IServiceProvider>

    public IServiceProvider Value { get => ServiceProvider; set => ServiceProvider = value; }
    IServiceProvider IReadWrapper<IServiceProvider>.Value => ServiceProvider;
    IServiceProvider IWriteWrapper<IServiceProvider>.Value { set => ServiceProvider = value; }

    #endregion

    #endregion

    #region IServiceProvider

    object? IServiceProvider.GetService(Type serviceType) => this.GetService(serviceType, null);

    #endregion

#if OLD
    #region GuaranteedInstanceProvider

    protected Func<Type, object> GuaranteedInstanceProvider(Func<Type, object> fallback) =>
        new Func<Type, object>(createType => GetService(createType) ?? fallback?.Invoke(createType));

    #endregion

#if UNUSED // DEPRECATED - use ServiceLocator.GetServiceOrSingleton instead
    private const bool DefaultCreateIfMissing = true;

    //public TInterface GetServiceOrSingleton<TInterface>(IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing, Func<TInterface> singletonFactory = null)
    //=> (TInterface)GetServiceOrSingleton(typeof(TInterface), serviceProvider, createIfMissing, singletonFactory: singletonFactory);

    //public TInterface GetServiceOrSingleton<TInterface, TConcrete>(IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing, Func<TInterface> singletonFactory = null)
    //    => (TInterface)GetServiceOrSingleton(typeof(TInterface), serviceProvider, createIfMissing, typeof(TConcrete),
    //        singletonFactory: (singletonFactory != null ? () => singletonFactory() : (Func<object>)null));

    //public TInterface GetServiceOrSingleton<TInterface>(IServiceProvider serviceProvider = null, bool createIfMissing = DefaultCreateIfMissing, Func<TInterface> singletonFactory = null)
    //    => (TInterface)GetServiceOrSingleton(typeof(TInterface), serviceProvider, createIfMissing, concreteType: null,
    //        singletonFactory: (singletonFactory != null ? () => singletonFactory() : (Func<object>)null));

    //private readonly bool UseManualSingletonServiceProvider = false;
    //public object GetServiceOrSingleton(Type serviceType) => GetServiceOrSingleton(serviceType, null, DefaultCreateIfMissing);

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

#endif

#if COMMENTED // REVIEW

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

#endif
#endif

}

public static class IServiceProviderX
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fallbackServiceProvider">can be null</param>
    /// <param name="tryFirstServiceProvider">can be null</param>
    /// <returns></returns>
    public static IEnumerable<T>? GetServices<T>(this IServiceProvider? fallbackServiceProvider, IServiceProvider? tryFirstServiceProvider = null)
        where T : class
    {
        var singleResult = tryFirstServiceProvider?.GetService<T>() ?? fallbackServiceProvider?.GetService<T>();
        if (singleResult != null)
        {
            return new T[] { singleResult };
        }

        var multiResult = tryFirstServiceProvider?.GetService<IEnumerable<T>>() ?? fallbackServiceProvider?.GetService<IEnumerable<T>>();
        return multiResult ?? Enumerable.Empty<T>();
    }
}

public static class IDependencyContextX
{
    #region TODO: Pass-thru to IServiceProviderX

    public static T? GetService<T>(this IDependencyContext dependencyContext, IServiceProvider? tryFirstServiceProvider = null)
        where T : class
        => (T?)dependencyContext.GetService(typeof(T), tryFirstServiceProvider);

    public static T GetRequiredService<T>(this IDependencyContext dependencyContext, IServiceProvider? tryFirstServiceProvider = null)
        => (T?)dependencyContext.GetService(typeof(T), tryFirstServiceProvider) ?? throw new DependencyMissingException(typeof(T).FullName);
        
    public static object? GetService(this IDependencyContext dependencyContext, Type serviceType, IServiceProvider? tryFirstServiceProvider = null)
    {
        object? result = null;

        #region Try IServiceProvider from Parameter

        if (tryFirstServiceProvider != null && !Object.ReferenceEquals(tryFirstServiceProvider, dependencyContext))
        {
            result = tryFirstServiceProvider.GetService(serviceType);
            if (result != null) { return result; }
        }

        #endregion

        #region Try this.ServiceProvider ?? SingleRootServiceProvider
        {
            var _serviceProvider = dependencyContext.ServiceProvider ?? DependencyContext.GlobalServiceProvider;
            if (_serviceProvider != null)
            {
                result = _serviceProvider.GetService(serviceType);
                if (result != null) { return result; }
            }
        }
        #endregion

        return result;
    }

    #endregion

    #region Pass-thru to IServiceProviderX

    [Obsolete("Prefer GetServices(this IServiceProvider, ...)")]
    public static IEnumerable<T>? GetServices<T>(this IDependencyContext dependencyContext, IServiceProvider? tryFirstServiceProvider = null)
        where T : class
    {
        return dependencyContext?.ServiceProvider?.GetServices<T>(tryFirstServiceProvider);

        // OLD
        //var singleResult = dependencyContext.GetService<T>(tryFirstServiceProvider);
        //return singleResult != null ? (new T[] { singleResult }) : dependencyContext.GetService<IEnumerable<T>>(); // Bug: didn't try tryFirstServiceProvider
    }

    #endregion

    public static void UseAsGuaranteedSingletonProvider(this IDependencyContext dependencyContext, bool useDefaultAsFallback = true)
    {
        var fallback = useDefaultAsFallback ? ManualSingletonProvider.GuaranteedInstanceProvider : null;
        ManualSingletonProvider.GuaranteedInstanceProvider =
                new Func<Type, object>(createType => dependencyContext.GetService(createType) ?? fallback?.Invoke(createType) ?? throw new DependencyMissingException(createType.FullName));

        //ManualSingletonProvider.GuaranteedInstanceProvider =
        //dc.GuaranteedInstanceProvider(fallback: useDefaultAsFallback ? ManualSingletonProvider.GuaranteedInstanceProvider : null);
    }

    // UNUSED
    //public static void UseAsGuaranteedSingletonProvider(this IServiceProvider serviceProvider, bool useDefaultAsFallback = true)
    //{
    //    var fallback = useDefaultAsFallback ? ManualSingletonProvider.GuaranteedInstanceProvider : null;
    //    ManualSingletonProvider.GuaranteedInstanceProvider =
    //            new Func<Type, object>(createType => serviceProvider.GetService(createType) ?? fallback?.Invoke(createType) ?? throw new DependencyMissingException(createType.FullName));
    //}
}
