using LionFire.DependencyInjection;
using LionFire.FlexObjects;
using LionFire.Ontology;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.FlexObjects.Services;


public class FlexServiceProvider : IServiceProvider
{
    #region Relationships

    public IFlex Flex { get; }

    #endregion

    #region Lifecycle

    public FlexServiceProvider(IFlex flex, Func<FlexServiceProvider, IServiceProvider?>? fallback = null)
    {
        Flex = flex;

        var FallbackServiceProviderFunc = fallback ?? DefaultStrategy;

        DynamicServiceProvider = new DynamicServiceProvider(() => FallbackServiceProviderFunc(this));
    }

    #endregion

    #region State

    public DynamicServiceProvider DynamicServiceProvider { get; }

    // CLEANUP
    //private IServiceProvider? resolvedServiceProvider;
    //public IServiceProvider? FallbackServiceProvider => FallbackServiceProviderFunc(this);
    //public Func<FlexServiceProvider, IServiceProvider?> FallbackServiceProviderFunc { get; set; }

    #region (static) Strategies

    public static Func<FlexServiceProvider, IServiceProvider?> DefaultStrategy { get; set; } = OneShotServiceProviderFromFlex;

    public static IServiceProvider? OneShotServiceProviderFromFlex(FlexServiceProvider @this)
    {
        var result = @this.Flex.Query<IServiceProvider>();
        if (result != null)
        {
            @this.DynamicServiceProvider.Parent = result;
            @this.DynamicServiceProvider.ParentFunc = null;
            // CLEANUP
            //@this.resolvedServiceProvider = result;
            //@this.FallbackServiceProviderFunc = ResolvedServiceProvider;
        }
        return result;
    }

    public static IServiceProvider? FallbackServiceProviderFromFlex(FlexServiceProvider @this)
        => @this.Flex.Query<IServiceProvider>();

    public static IServiceProvider? FallbackServiceProviderFromFlexRecursive(FlexServiceProvider @this)
        => @this.Flex.RecursiveQuery<IServiceProvider>();

    // CLEANUP
    //public static IServiceProvider? ResolvedServiceProvider(FlexServiceProvider @this)
    //    //=> @this.resolvedServiceProvider;
    //    => @this.DynamicServiceProvider.Parent;

    #endregion

    #endregion

    #region IServiceProvider: Pass-thru

    public object? GetService(Type serviceType) => DynamicServiceProvider.GetService(serviceType);

    #endregion
}
