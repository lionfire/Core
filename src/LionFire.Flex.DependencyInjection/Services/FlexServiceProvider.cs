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

    #region (static) Strategies

    public static Func<FlexServiceProvider, IServiceProvider?> DefaultStrategy { get; set; } = OneShotServiceProviderFromFlex;

    public static IServiceProvider? OneShotServiceProviderFromFlex(FlexServiceProvider @this)
    {
        var result = @this.Flex.Query<IServiceProvider>();
        if (result != null)
        {
            @this.DynamicServiceProvider.Parent = result;
            @this.DynamicServiceProvider.ParentFunc = null;
        }
        return result;
    }

    public static IServiceProvider? FallbackServiceProviderFromFlex(FlexServiceProvider @this)
        => @this.Flex.Query<IServiceProvider>();

    public static IServiceProvider? FallbackServiceProviderFromFlexRecursive(FlexServiceProvider @this)
        => @this.Flex.RecursiveQuery<IServiceProvider>();

    #endregion

    #endregion

    #region IServiceProvider: Pass-thru

    public object? GetService(Type serviceType) => DynamicServiceProvider.GetService(serviceType);

    #endregion
}
