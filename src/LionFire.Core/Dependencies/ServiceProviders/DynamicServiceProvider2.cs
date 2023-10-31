#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Dependencies;

// REVIEW - still used?  TODO - merge/reconcile with DynamicServiceProvider

/// <summary>
/// Implements IServiceProvider, allowing for ongoing dynamic configuration. 
/// 
/// Rather than implementing IServiceCollection, there is a simpler implementation.
/// 
/// Supports:
///  - Singletons
///  - Transients
///  - Falls back to a set of other IServiceProviders
///  - Falls back to a single optional parent IServiceProvider
///  
/// TODO:
///  - Scoped
/// </summary>
public class DynamicServiceProvider2 : IServiceProvider
{

    /// <summary>
    /// If true, discard registrations when no longer needed
    /// </summary>
    public static bool Cleanup { get; set; } = true;

    #region Dependencies

    public IServiceProvider? ParentServiceProvider { get; }

    #endregion

    #region Parameters
    

    public Dictionary<Type, object> Singletons { get; } = new Dictionary<Type, object>();
    public Dictionary<Type, Type> SingletonRegistrations { get; } = new Dictionary<Type, Type>();

    public Dictionary<Type, Type> TransientRegistrations { get; } = new Dictionary<Type, Type>();

    public List<IServiceProvider> ServiceProviders { get; } = new List<IServiceProvider>();
    //public Func<IEnumerable<IServiceProvider>> ServiceProvidersFunc { get; set; } = () => Enumerable.Empty<IServiceProvider>();

    #endregion

    #region Lifecycle

    public DynamicServiceProvider2(IServiceProvider? parentServiceProvider = null)
    {
        ParentServiceProvider = parentServiceProvider;
        //ServiceProvidersFunc = () => ServiceProviders;
    }

    #endregion

    #region (public) Methods

    public object? GetService(Type serviceType)
    {
        #region Singleton

        if (Singletons.ContainsKey(serviceType)) { return Singletons[serviceType]; }

        if (SingletonRegistrations.ContainsKey(serviceType)) {
            var newSingleton = ActivatorUtilities.CreateInstance(this, serviceType);
            Singletons.Add(serviceType, newSingleton);
            if (Cleanup) { SingletonRegistrations.Remove(serviceType); }
            return newSingleton;
        }

        #endregion

        #region Transient

        if (TransientRegistrations.ContainsKey(serviceType)) { return ActivatorUtilities.CreateInstance(this, serviceType); }

        #endregion

        #region Fallback ServiceProviders

        foreach (var sp in ServiceProviders)
        {
            var result = sp.GetService(serviceType);
            if (result != null)
            {
                return result;
            }
        }

        #endregion

        return ParentServiceProvider?.GetService(serviceType);
    }

    #endregion

    #region Method

    // ENH: Verify doesn't exist in SingletonRegistrations or TransientRegistrations
    public void AddSingleton<T>(T instance) => Singletons.Add(typeof(T), instance ?? throw new ArgumentNullException(nameof(instance)));

    // ENH: Verify doesn't exist in Singletons or TransientRegistrations
    public void AddSingleton<TConcrete>() => SingletonRegistrations.Add(typeof(TConcrete), typeof(TConcrete));
    //public bool RemoveSingleton<TConcrete>() => SingletonRegistrations.Remove(typeof(TConcrete));

    // ENH: Verify doesn't exist in Singletons or TransientRegistrations
    public void AddSingleton<TInterface, TConcrete>() => SingletonRegistrations.Add(typeof(TInterface), typeof(TConcrete));

    // ENH: Verify doesn't exist in SingletonRegistrations or TransientRegistrations
    public void AddSingleton(Type serviceType, object instance) => Singletons.Add(serviceType, instance);

    // ENH: Verify doesn't exist in SingletonRegistrations or Singletons
    public void AddTransient<TService, TImplementation>() => Singletons.Add(typeof(TService), typeof(TImplementation));

    #endregion

}
